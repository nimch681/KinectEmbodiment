//   Copyright (C) 2015  Marek Kowalski (M.Kowalski@ire.pw.edu.pl), Jacek Naruniec (J.Naruniec@ire.pw.edu.pl)
//   License: MIT Software License   See LICENSE.txt for the full license.

//   If you use this software in your research, then please use the following citation:

//    Kowalski, M.; Naruniec, J.; Daniluk, M.: "LiveScan3D: A Fast and Inexpensive 3D Data
//    Acquisition System for Multiple Kinect v2 Sensors". in 3D Vision (3DV), 2015 International Conference on, Lyon, France, 2015

//    @INPROCEEDINGS{Kowalski15,
//        author={Kowalski, M. and Naruniec, J. and Daniluk, M.},
//        booktitle={3D Vision (3DV), 2015 International Conference on},
//        title={LiveScan3D: A Fast and Inexpensive 3D Data Acquisition System for Multiple Kinect v2 Sensors},
//        year={2015},
//    }
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.Serialization;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Timers;

using System.Diagnostics;


namespace KinectServer
{
    public partial class MainWindowForm : Form
    {
        [DllImport("ICP.dll")]
        static extern float ICP(IntPtr verts1, IntPtr verts2, int nVerts1, int nVerts2, float[] R, float[] t, int maxIter = 200);

        KinectServer oServer;
        TransferServer oTransferServer;

        //Those three variables are shared with the OpenGLWindow class and are used to exchange data with it.
        //Vertices from all of the sensors
        List<float> lAllVertices = new List<float>();
        //Color data from all of the sensors
        List<byte> lAllColors = new List<byte>();
        //Sensor poses from all of the sensors
        List<AffineTransform> lAllCameraPoses = new List<AffineTransform>();
        //Body data from all of the sensors
        List<Body> lAllBodies = new List<Body>();

        float size;

        bool bServerRunning = false;
        bool bRecording = false;
        bool bSaving = false;

        //Live view open or not
        bool bLiveViewRunning = false;

        System.Timers.Timer oStatusBarTimer = new System.Timers.Timer();

        KinectSettings oSettings = new KinectSettings();
        //The live view window class
        OpenGLWindow oOpenGLWindow;

        public MainWindowForm()
        {
            //This tries to read the settings from "settings.bin", if it failes the settings stay at default values.
            try
            {
                IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                Stream stream = new FileStream("settings.bin", FileMode.Open, FileAccess.Read);
                oSettings = (KinectSettings)formatter.Deserialize(stream);
                stream.Close();
            }
            catch(Exception)
            {
            }

            oServer = new KinectServer(oSettings);
            oServer.eSocketListChanged += new SocketListChangedHandler(UpdateListView);
            oTransferServer = new TransferServer();
            oTransferServer.lVertices = lAllVertices;
            oTransferServer.lColors = lAllColors;
            oTransferServer.IBodies = lAllBodies;//just added this 
            
            
            

            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //The current settings are saved to a files.
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            Stream stream = new FileStream("settings.bin", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, oSettings);
            stream.Close();

            oServer.StopServer();
            oTransferServer.StopServer();
        }

        //Starts the server
        private void btStart_Click(object sender, EventArgs e)
        {
            bServerRunning = !bServerRunning;

            if (bServerRunning)
            {
                oServer.StartServer();
                oTransferServer.StartServer();
                btStart.Text = "Stop server";
            }
            else
            {
                oServer.StopServer();
                oTransferServer.StopServer();
                btStart.Text = "Start server";
            }
        }

        //Opens the settings form
        private void btSettings_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.oSettings = oSettings;
            form.oServer = oServer;
            form.Show();            
        }

        //Performs recording which is synchronized frame capture.
        //The frames are downloaded from the clients and saved once recording is finished.
        private void recordingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            oServer.ClearStoredFrames();

            int nCaptured = 0;
            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                oServer.CaptureSynchronizedFrame();

                nCaptured++;
                SetStatusBarOnTimer("Captured frame " + (nCaptured).ToString() + ".", 5000);
            }
        }

        private void recordingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //After recording has been terminated it is time to begin saving the frames.
            //Saving is downloading the frames from clients and saving them locally.
            bSaving = true;
            
            btRecord.Text = "Stop saving";
            btRecord.Enabled = true;

            savingWorker.RunWorkerAsync();
        }

        //Opens the live view window
        private void OpenGLWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bLiveViewRunning = true;
            oOpenGLWindow = new OpenGLWindow();

            //The variables below are shared between this class and the OpenGLWindow.
            lock (lAllVertices)
            {
                oOpenGLWindow.vertices = lAllVertices;
                oOpenGLWindow.colors = lAllColors;
                oOpenGLWindow.cameraPoses = lAllCameraPoses;
                oOpenGLWindow.bodies = lAllBodies;
                oOpenGLWindow.settings = oSettings;
            }
            oOpenGLWindow.Run();
        }

        private void OpenGLWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bLiveViewRunning = false;
            updateWorker.CancelAsync();
        }

        private void savingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int nFrames = 0;

            string outDir = "out" + "\\" + txtSeqName.Text + "\\";
            DirectoryInfo di = Directory.CreateDirectory(outDir);

            BackgroundWorker worker = (BackgroundWorker)sender;
            //This loop is running till it is either cancelled (using the btRecord button), or till there are no more stored frames.
            while (!worker.CancellationPending)
            {
                List<List<byte>> lFrameRGBAllDevices = new List<List<byte>>();
                List<List<float>> lFrameVertsAllDevices = new List<List<float>>();

                bool success = oServer.GetStoredFrame(lFrameRGBAllDevices, lFrameVertsAllDevices);

                //This indicates that there are no more stored frames.
                if (!success)
                    break;

                nFrames++;
                int nVerticesTotal = 0;
                for (int i = 0; i < lFrameRGBAllDevices.Count; i++)
                {
                    nVerticesTotal += lFrameVertsAllDevices[i].Count;
                }

                List<byte> lFrameRGB = new List<byte>();
                List<Single> lFrameVerts = new List<Single>();

                SetStatusBarOnTimer("Saving frame " + (nFrames).ToString() + ".", 5000);
                for (int i = 0; i < lFrameRGBAllDevices.Count; i++)
                {                                 
                    lFrameRGB.AddRange(lFrameRGBAllDevices[i]);
                    lFrameVerts.AddRange(lFrameVertsAllDevices[i]);

                    //This is ran if the frames from each client are to be placed in separate files.
                    if (!oSettings.bMergeScansForSave)
                    {
                        string outputFilename = outDir + "\\" + nFrames.ToString().PadLeft(5, '0') + i.ToString() + ".ply";
                        Utils.saveToPly(outputFilename, lFrameVertsAllDevices[i], lFrameRGBAllDevices[i], oSettings.bSaveAsBinaryPLY);                        
                    }
                }

                //This is ran if the frames from all clients are to be placed in a single file.
                if (oSettings.bMergeScansForSave)
                {
                    string outputFilename = outDir + "\\" + nFrames.ToString().PadLeft(5, '0') + ".ply";
                    Utils.saveToPly(outputFilename, lFrameVerts, lFrameRGB, oSettings.bSaveAsBinaryPLY);
                }
            }
        }

        private void savingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            oServer.ClearStoredFrames();
            bSaving = false;

            //If the live view window was open, we need to restart the UpdateWorker.
            if (bLiveViewRunning)
                RestartUpdateWorker();

            btRecord.Enabled = true;
            btRecord.Text = "Start recording";
            btRefineCalib.Enabled = true;
            btCalibrate.Enabled = true;
        }

        //Continually requests frames that will be displayed in the live view window.
        private void updateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<List<byte>> lFramesRGB = new List<List<byte>>();
            List<List<Single>> lFramesVerts = new List<List<Single>>();
            List<List<Body>> lFramesBody = new List<List<Body>>();

            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                Thread.Sleep(1);
                oServer.GetLatestFrame(lFramesRGB, lFramesVerts, lFramesBody);

                //Update the vertex and color lists that are common between this class and the OpenGLWindow.
                lock (lAllVertices)
                {
                    lAllVertices.Clear();
                    lAllColors.Clear();
                    lAllBodies.Clear();
                    lAllCameraPoses.Clear();

   

                    List<Single> HeadX = new List<Single>();
                    List<Single> HeadY = new List<Single>();
                    List<Single> HeadZ = new List<Single>();

                    

                    List<Single> UpperabdomenX = new List<Single>();
                    List<Single> UpperabdomenY = new List<Single>();
                    List<Single> UpperabdomenZ = new List<Single>();

                    List<Single> LegX = new List<Single>();
                    List<Single> LegY = new List<Single>();
                    List<Single> LegZ = new List<Single>();



                    List<Single> BodyVert = new List<Single>();
                    List<byte> BodyRGB = new List<byte>();

                    List<Single> BodyX = new List<Single>();
                    List<Single> BodyY = new List<Single>();
                    List<Single> BodyZ = new List<Single>();

                    List<Single> TempVerts = new List<Single>();
                    List<byte> TempRGB = new List<byte>();
                   
                    for (int i = 0; i < lFramesRGB.Count; i++)
                    {
                        TempVerts.AddRange(lFramesVerts[i]);
                        TempRGB.AddRange(lFramesRGB[i]);
                        lAllBodies.AddRange(lFramesBody[i]);                       
                    }


                    /**body segmentation starts here*/

                    for (int i = 0; i < 6; i++)
                    {

                        if (lAllBodies[i].bTracked)
                        {


                            int pdidx = 0;
                            int nAllVertices = TempVerts.Count;
                            while (pdidx < nAllVertices)
                            {
                                float ShoulderY = lAllBodies[i].lJoints[20].position.Y;
                                float SpineBaseY = lAllBodies[i].lJoints[0].position.Y;

                                
                                BodySegmentWithVerticesArray(BodyVert, BodyRGB, lAllBodies, i, 0, TempVerts, TempRGB, pdidx, 1f, 1f, 1f, 1f, 1f, 1f, BodyX, BodyY, BodyZ);
                                pdidx += 3;

                            }



                        }

                        if (size != 0)
                        {
                            scale(lAllBodies, i, BodyVert, size, 1f, size, BodyX, BodyY, BodyZ);
                        }
                        
                    }


                    
                    lAllVertices.AddRange(BodyVert);
                    lAllColors.AddRange(BodyRGB);
                  

                    lAllCameraPoses.AddRange(oServer.lCameraPoses);
                }

               
                

                //Notes the fact that a new frame was downloaded, this is used to estimate the FPS.
                if (oOpenGLWindow != null)
                    oOpenGLWindow.CloudUpdateTick();            
            }
        }
        
        //Performs the ICP based pose refinement.
        private void refineWorker_DoWork(object sender, DoWorkEventArgs e)
        {                      
            if (oServer.bAllCalibrated == false)
            {
                SetStatusBarOnTimer("Not all of the devices are calibrated.", 5000);
                return;
            } 

            //Download a frame from each client.
            List<List<float>> lAllFrameVertices = new List<List<float>>();
            List<List<byte>> lAllFrameColors = new List<List<byte>>();
            List<List<Body>> lAllFrameBody = new List<List<Body>>();
            oServer.GetLatestFrame(lAllFrameColors, lAllFrameVertices, lAllFrameBody);

            //Initialize containers for the poses.
            List<float[]> Rs = new List<float[]>();
            List<float[]> Ts = new List<float[]>();
            for (int i = 0; i < lAllFrameVertices.Count; i++)
            {
                float[] tempR = new float[9];
                float[] tempT = new float[3];
                for (int j = 0; j < 3; j++)
                {
                    tempT[j] = 0;
                    tempR[j + j * 3] = 1;
                }

                Rs.Add(tempR);
                Ts.Add(tempT);
            }

            //Use ICP to refine the sensor poses.
            //This part is explained in more detail in our article (name on top of this file).

            for (int refineIter = 0; refineIter < oSettings.nNumRefineIters; refineIter++)
            {
                for (int i = 0; i < lAllFrameVertices.Count; i++)
                {
                    List<float> otherFramesVertices = new List<float>();
                    for (int j = 0; j < lAllFrameVertices.Count; j++)
                    {
                        if (j == i)
                            continue;
                        otherFramesVertices.AddRange(lAllFrameVertices[j]);
                    }

                    float[] verts1 = otherFramesVertices.ToArray();
                    float[] verts2 = lAllFrameVertices[i].ToArray();

                    IntPtr pVerts1 = Marshal.AllocHGlobal(otherFramesVertices.Count * sizeof(float));
                    IntPtr pVerts2 = Marshal.AllocHGlobal(lAllFrameVertices[i].Count * sizeof(float));

                    Marshal.Copy(verts1, 0, pVerts1, verts1.Length);
                    Marshal.Copy(verts2, 0, pVerts2, verts2.Length);

                    ICP(pVerts1, pVerts2, otherFramesVertices.Count / 3, lAllFrameVertices[i].Count / 3, Rs[i], Ts[i], oSettings.nNumICPIterations);

                    Marshal.Copy(pVerts2, verts2, 0, verts2.Length);
                    lAllFrameVertices[i].Clear();
                    lAllFrameVertices[i].AddRange(verts2);
                }
            }

            //Update the calibration data in client machines.
            List<AffineTransform> worldTransforms = oServer.lWorldTransforms;
            List<AffineTransform> cameraPoses = oServer.lCameraPoses;

            for (int i = 0; i < worldTransforms.Count; i++)
            {
                float[] tempT = new float[3];
                float[,] tempR = new float[3, 3];
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        tempT[j] += Ts[i][k] * worldTransforms[i].R[k, j];
                    }

                    worldTransforms[i].t[j] += tempT[j];
                    cameraPoses[i].t[j] += Ts[i][j];
                }

                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            tempR[j, k] += Rs[i][l * 3 + j] * worldTransforms[i].R[l, k];
                        }

                        worldTransforms[i].R[j, k] = tempR[j, k];
                        cameraPoses[i].R[j, k] = tempR[j, k];
                    }
                }                    
            }

            oServer.lWorldTransforms = worldTransforms;
            oServer.lCameraPoses = cameraPoses;

            oServer.SendCalibrationData();
        }

        private void refineWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Re-enable all of the buttons after refinement.
            btRefineCalib.Enabled = true;
            btCalibrate.Enabled = true;
            btRecord.Enabled = true;
        }

        //This is used for: starting/stopping the recording worker, stopping the saving worker
        private void btRecord_Click(object sender, EventArgs e)
        {
            if (oServer.nClientCount < 1)
            {
                SetStatusBarOnTimer("At least one client needs to be connected for recording.", 5000);                
                return;
            }

            //If we are saving frames right now, this button stops saving.
            if (bSaving)
            {
                btRecord.Enabled = false;
                savingWorker.CancelAsync();
                return;
            }

            bRecording = !bRecording;
            if (bRecording)
            {
                //Stop the update worker to reduce the network usage (provides better synchronization).
                updateWorker.CancelAsync();

                recordingWorker.RunWorkerAsync();
                btRecord.Text = "Stop recording";
                btRefineCalib.Enabled = false;
                btCalibrate.Enabled = false;
            }
            else 
            {
                btRecord.Enabled = false;
                recordingWorker.CancelAsync();                
            }
            
        }

        private void btCalibrate_Click(object sender, EventArgs e)
        {
            oServer.Calibrate();
        }

        private void btRefineCalib_Click(object sender, EventArgs e)
        {
            if (oServer.nClientCount < 2)
            {
                SetStatusBarOnTimer("To refine calibration you need at least 2 connected devices.", 5000);
                return;
            }

            btRefineCalib.Enabled = false;
            btCalibrate.Enabled = false;
            btRecord.Enabled = false;

            refineWorker.RunWorkerAsync();
        }

        void RestartUpdateWorker()
        {
            if (!updateWorker.IsBusy)
                updateWorker.RunWorkerAsync();
        }

        private void btShowLive_Click(object sender, EventArgs e)
        {            
            RestartUpdateWorker();

            //Opens the live view window if it is not open yet.
            if (!OpenGLWorker.IsBusy)
                OpenGLWorker.RunWorkerAsync();
        }

        private void SetStatusBarOnTimer(string message, int milliseconds)
        {
            statusLabel.Text = message;

            oStatusBarTimer.Stop();
            oStatusBarTimer = new System.Timers.Timer();

            oStatusBarTimer.Interval = milliseconds;
            oStatusBarTimer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
            {
                oStatusBarTimer.Stop();
                statusLabel.Text = "";
            };
            oStatusBarTimer.Start();
        }

        //Updates the ListBox contaning the connected clients, called by events inside KinectServer.
        private void UpdateListView(List<KinectSocket> socketList)
        {
            List<string> listBoxItems = new List<string>();

            for (int i = 0; i < socketList.Count; i++)
                listBoxItems.Add(socketList[i].sSocketState);

            lClientListBox.DataSource = listBoxItems;
        }


        public void BodySegmentWithVerticesArray(List<Single> BodyPartVert, List<byte> BodyPartRGB, List<Body> body, int bodyIndex, int JointIndex,
            List<Single> vertArray, List<byte> RGBArray, int vertIndex, float mixX, float maxX, float minY, float maxY, float minZ, float maxZ, List<Single> Xlist, List<Single> Ylist, List<Single> Zlist)
        {


            if (body[bodyIndex].bTracked)
            {
                //Console.Write("This is working bodyIndex " + bodyIndex);
                float jointX = lAllBodies[bodyIndex].lJoints[JointIndex].position.X;
                float jointY = lAllBodies[bodyIndex].lJoints[JointIndex].position.Y;
                float jointZ = lAllBodies[bodyIndex].lJoints[JointIndex].position.Z;

                // Console.Write("This is working Joint X " + jointX + "\n");

                if (vertArray[vertIndex] <= jointX + maxX && vertArray[vertIndex] >= jointX - mixX)
                {
                    //Console.Write("This is working vertIndex " + vertIndex + "\n");

                    if (vertArray[vertIndex + 1] <= jointY + maxY && vertArray[vertIndex + 1] >= jointY - minY)
                    {
                        // Console.Write("This is working vertIndex " + vertIndex + "\n");

                        if (vertArray[vertIndex + 2] <= jointZ + maxZ && vertArray[vertIndex + 2] >= jointZ - minZ)
                        {


                            BodyPartVert.Add(vertArray[vertIndex]);
                            BodyPartVert.Add(vertArray[vertIndex + 1]);
                            BodyPartVert.Add(vertArray[vertIndex + 2]);
                            Xlist.Add(vertArray[vertIndex]);
                            Ylist.Add(vertArray[vertIndex + 1]);
                            Zlist.Add(vertArray[vertIndex + 2]);
                            BodyPartRGB.Add(RGBArray[vertIndex]);
                            BodyPartRGB.Add(RGBArray[vertIndex + 1]);
                            BodyPartRGB.Add(RGBArray[vertIndex + 2]);

                        }

                    }

                }
            }

        }

        public void BodySegment(List<Single> BodyPartVert, List<byte> BodyPartRGB, List<Body> body, int bodyIndex, int JointIndex,
            List<Single> vertArray, List<byte> RGBArray, int vertIndex, float mixX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {


            if (body[bodyIndex].bTracked)
            {
                //Console.Write("This is working bodyIndex " + bodyIndex);
                float jointX = lAllBodies[bodyIndex].lJoints[JointIndex].position.X;
                float jointY = lAllBodies[bodyIndex].lJoints[JointIndex].position.Y;
                float jointZ = lAllBodies[bodyIndex].lJoints[JointIndex].position.Z;

                // Console.Write("This is working Joint X " + jointX + "\n");


                if (vertArray[vertIndex] <= jointX + maxX && vertArray[vertIndex] >= jointX - mixX)
                {
                    //Console.Write("This is working vertIndex " + vertIndex + "\n");

                    if (vertArray[vertIndex + 1] <= jointY + maxY && vertArray[vertIndex + 1] >= jointY - minY)
                    {
                        // Console.Write("This is working vertIndex " + vertIndex + "\n");

                        if (vertArray[vertIndex + 2] <= jointZ + maxZ && vertArray[vertIndex + 2] >= jointZ - minZ)
                        {


                            BodyPartVert.Add(vertArray[vertIndex]);
                            BodyPartVert.Add(vertArray[vertIndex + 1]);
                            BodyPartVert.Add(vertArray[vertIndex + 2]);

                            BodyPartRGB.Add(RGBArray[vertIndex]);
                            BodyPartRGB.Add(RGBArray[vertIndex + 1]);
                            BodyPartRGB.Add(RGBArray[vertIndex + 2]);

                        }

                    }

                }
            }

        }

        public void BodySegmentWithAbsoluteMaxY(List<Single> BodyPartVert, List<byte> BodyPartRGB, List<Body> body, int bodyIndex, int JointIndex,
            List<Single> vertArray, List<byte> RGBArray, int vertIndex, float mixX, float maxX, float minY, float maxY, float minZ, float maxZ, List<Single> Xlist, List<Single> Ylist, List<Single> Zlist)
        {


            if (body[bodyIndex].bTracked)
            {
                //Console.Write("This is working bodyIndex " + bodyIndex);
                float jointX = lAllBodies[bodyIndex].lJoints[JointIndex].position.X;
                float jointY = lAllBodies[bodyIndex].lJoints[JointIndex].position.Y;
                float jointZ = lAllBodies[bodyIndex].lJoints[JointIndex].position.Z;

                // Console.Write("This is working Joint X " + jointX + "\n");


                if (vertArray[vertIndex] <= jointX + maxX && vertArray[vertIndex] >= jointX - mixX)
                {
                    //Console.Write("This is working vertIndex " + vertIndex + "\n");

                    if (vertArray[vertIndex + 1] < maxY && vertArray[vertIndex + 1] >= jointY - minY)
                    {
                        // Console.Write("This is working vertIndex " + vertIndex + "\n");

                        if (vertArray[vertIndex + 2] <= jointZ + maxZ && vertArray[vertIndex + 2] >= jointZ - minZ)
                        {


                            BodyPartVert.Add(vertArray[vertIndex]);
                            BodyPartVert.Add(vertArray[vertIndex + 1]);
                            BodyPartVert.Add(vertArray[vertIndex + 2]);
                            Xlist.Add(vertArray[vertIndex]);
                            Ylist.Add(vertArray[vertIndex + 1]);
                            Zlist.Add(vertArray[vertIndex + 2]);
                            BodyPartRGB.Add(RGBArray[vertIndex]);
                            BodyPartRGB.Add(RGBArray[vertIndex + 1]);
                            BodyPartRGB.Add(RGBArray[vertIndex + 2]);

                        }

                    }

                }
            }

        }

        public void BodySegmentWithAbsoluteMinY(List<Single> BodyPartVert, List<byte> BodyPartRGB, List<Body> body, int bodyIndex, int JointIndex,
           List<Single> vertArray, List<byte> RGBArray, int vertIndex, float mixX, float maxX, float minY, float maxY, float minZ, float maxZ, List<Single> Xlist, List<Single> Ylist, List<Single> Zlist)
        {


            if (body[bodyIndex].bTracked)
            {
                //Console.Write("This is working bodyIndex " + bodyIndex);
                float jointX = lAllBodies[bodyIndex].lJoints[JointIndex].position.X;
                float jointY = lAllBodies[bodyIndex].lJoints[JointIndex].position.Y;
                float jointZ = lAllBodies[bodyIndex].lJoints[JointIndex].position.Z;

                // Console.Write("This is working Joint X " + jointX + "\n");


                if (vertArray[vertIndex] <= jointX + maxX && vertArray[vertIndex] >= jointX - mixX)
                {
                    //Console.Write("This is working vertIndex " + vertIndex + "\n");

                    if (vertArray[vertIndex + 1] < maxY && vertArray[vertIndex + 1] >= minY)
                    {
                        // Console.Write("This is working vertIndex " + vertIndex + "\n");

                        if (vertArray[vertIndex + 2] <= jointZ + maxZ && vertArray[vertIndex + 2] >= jointZ - minZ)
                        {


                            BodyPartVert.Add(vertArray[vertIndex]);
                            BodyPartVert.Add(vertArray[vertIndex + 1]);
                            BodyPartVert.Add(vertArray[vertIndex + 2]);
                            Xlist.Add(vertArray[vertIndex]);
                            Ylist.Add(vertArray[vertIndex + 1]);
                            Zlist.Add(vertArray[vertIndex + 2]);
                            BodyPartRGB.Add(RGBArray[vertIndex]);
                            BodyPartRGB.Add(RGBArray[vertIndex + 1]);
                            BodyPartRGB.Add(RGBArray[vertIndex + 2]);

                        }

                    }

                }
            }

        }

        public void scale(List<Body> body, int bodyIndex, List<Single> vertices, float scaleFactorX, float scaleFactorY, float scaleFactorZ, List<Single> Xlist, List<Single> Ylist, List<Single> Zlist)
        {

            if (body[bodyIndex].bTracked)
            {

                float centerX = Center(Xlist);
                float centerY = Center(Ylist);
                float centerZ = Center(Zlist);

                //Console.Write("The center is " + " X: " + centerX + " Y: " + centerY + " Z: " + centerZ + " \n");
                int verticesToread = 0;
                while (verticesToread < vertices.Count)
                {
                    vertices[verticesToread] = ((vertices[verticesToread] - centerX) * scaleFactorX) + centerX;
                    vertices[verticesToread + 1] = ((vertices[verticesToread + 1] - centerY) * scaleFactorY) + centerY;
                    vertices[verticesToread + 2] = ((vertices[verticesToread + 2] - centerZ) * scaleFactorZ) + centerZ;

                    verticesToread += 3;
                }
            }

        }

        public float sum(List<Single> co)
        {
            float result = 0;

            for (int i = 0; i < co.Count; i++)
            {
                result += co[i];
            }

            return result;
        }

        public float Average(List<Single> co)
        {
            float s = sum(co);
            float result = (float)s / co.Count;
            return result;
        }

        public float Center(List<Single> co)
        {
            return Average(co);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {


            if (trackBar1.Value == 0)
            {
                size = 0.7f;
            }
            if (trackBar1.Value == 1)
            {
                size = 0.8f;
            }
            if (trackBar1.Value == 2)
            {

                size = 0.9f;
            }
            if (trackBar1.Value == 3)
            {
                size = 1f;
            }
            if (trackBar1.Value == 4)
            {
                size = 1.1f;

            }
            if (trackBar1.Value == 5)
            {
                size = 1.2f;
            }
            if (trackBar1.Value == 6)
            {
                size = 1.3f;
            }
            if (trackBar1.Value == 7)
            {
                size = 1.4f;
            }
            if (trackBar1.Value == 8)
            {
                size = 1.5f;
            }
            if (trackBar1.Value == 9)
            {
                size = 1.6f;
            }
            if(trackBar1.Value == 10)
            {
                size = 1.7f;
            }
        }
    }
}
