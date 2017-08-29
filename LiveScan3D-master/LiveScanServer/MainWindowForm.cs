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

        List<Single> SizePoint = new List<Single>();
        List<byte> SizeRGB = new List<byte>();

        List<float> sizelist = new List<float>();
        Stack<int> randomNumberIndex = new Stack<int>();
        Stack<int> similarRandomNumberIndex = new Stack<int>();
        List<String> RowInScales = new List<String>();
        List<String> RowInAnswers = new List<String>();
        CsvFileWriter csv_scales;
        CsvFileWriter csv_answers;

        //List<int> randomNumberIndex = new List<int>();

        float size;



        bool bServerRunning = false;
        bool bRecording = false;
        bool bSaving = false;

        //Live view open or not

        bool bLiveViewRunning = false;

        private bool BodyView_IsOn;

        public bool BodyViewIsOn
        {
            get
            {
                return BodyView_IsOn;
            }
            set
            {
                BodyView_IsOn = value;
                BodyView.Text = BodyView_IsOn ? "Resizing On" : "Resizing Off";
            }
        }

        private bool Stop_IsOn;
        public bool StopIsOn
        {
            get
            {
                return Stop_IsOn;
            }
            set
            {
                Stop_IsOn = value;
                Stop.Text = Stop_IsOn ? "Stop On" : "Stop Off";
            }
        }

        private bool Scoll_IsOn;
        public bool ScollIsOn
        {
            get
            {
                return Scoll_IsOn;
            }
            set
            {
                Scoll_IsOn = value;
                ScollDis.Text = Scoll_IsOn ? "Ideal Size On" : "Ideal Size Off";
                groupBox1.Enabled = ScollIsOn;
            }
        }

        private bool SimilarBody_IsOn;
        public bool SimilarBodyIsOn
        {
            get
            {
                return SimilarBody_IsOn;
            }
            set
            {
                SimilarBody_IsOn = value;
                SimilarBody.Text = SimilarBody_IsOn ? "Similar Size On" : "Similar Size Off";
                groupBox3.Enabled = SimilarBodyIsOn;
            }
        }

        private bool randbutton_IsOn;
        public bool randbuttonIsOn
        {
            get
            {
                return randbutton_IsOn;
            }
            set
            {
                randbutton_IsOn = value;
                RandomBodiesArray.Text = randbutton_IsOn ? "Body Array is Not empty" : "Body Array is empty";
                groupBox4.Enabled = randbuttonIsOn;
            }
        }

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
            SizeRGB.Add(201);
            SizeRGB.Add(201);
            SizeRGB.Add(201);
            SizePoint.Add(0.005f);
            SizePoint.Add(0.005f);
            SizePoint.Add(0.005f);
            sizelist.Add(0.6f);
            sizelist.Add(0.7f);
            sizelist.Add(0.8f);
            sizelist.Add(0.9f);
            sizelist.Add(1f);
            sizelist.Add(1.1f);
            sizelist.Add(1.2f);
            sizelist.Add(1.3f);
            sizelist.Add(1.4f);
           
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

   
                    

                    List<Single> BodyVert = new List<Single>();
                    List<byte> BodyRGB = new List<byte>();

                    List<Single> BodyX = new List<Single>();
                    List<Single> BodyY = new List<Single>();
                    List<Single> BodyZ = new List<Single>();
                    List<float> BodyCenters = new List<float>(3);
                    List<Single> TempVerts = new List<Single>();
                    List<byte> TempRGB = new List<byte>();

                    

                    
 
                    for (int i = 0; i < lFramesRGB.Count; i++)
                    {
                        TempVerts.AddRange(lFramesVerts[i]);
                        TempRGB.AddRange(lFramesRGB[i]);
                        lAllBodies.AddRange(lFramesBody[i]);                       
                    }
                    if (BodyViewIsOn == true)
                    {
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

                                    BodySegmentWithVerticesArray(BodyVert, BodyRGB, lAllBodies, i, 0, TempVerts, TempRGB, pdidx, 1f, 1f, 1f, 1f, 1f, 1f);

                                    pdidx += 3;
                                }

                            }


                            roationX(lAllBodies, i, BodyVert, 33, BodyX, BodyY, BodyZ, BodyCenters);                                       
                            scale(lAllBodies, i, BodyVert, size, 1f, size, BodyCenters);

                        }


                        CheckPointSize();
                        lAllVertices.AddRange(SizePoint);
                        lAllColors.AddRange(SizeRGB);

                        if (StopIsOn == false)
                        {

                           
                            lAllVertices.AddRange(BodyVert);
                            lAllColors.AddRange(BodyRGB);
                            
                        }
                    }
                    else {
                        if(StopIsOn == false) {
                            CheckPointSize();
                            lAllVertices.AddRange(SizePoint);
                            lAllColors.AddRange(SizeRGB);
                            lAllVertices.AddRange(TempVerts);
                            lAllColors.AddRange(TempRGB);
                           
                        }
                        
                    }
                    
                   

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

       
        public void scale(List<Body> body, int bodyIndex, List<Single> vertices, float scaleFactorX, float scaleFactorY, float scaleFactorZ, List<float> Centers)
        {

            if (body[bodyIndex].bTracked)
            {

               
                int verticesToread = 0;
                while (verticesToread < vertices.Count)
                {
                    vertices[verticesToread] = ((vertices[verticesToread] - Centers[0]) * scaleFactorX) + Centers[0];
                    vertices[verticesToread + 1] = ((vertices[verticesToread + 1] - Centers[1]) * scaleFactorY) + Centers[1];
                    vertices[verticesToread + 2] = ((vertices[verticesToread + 2] - Centers[2]) * scaleFactorZ) + Centers[2];

                    verticesToread += 3;
                }
            }

        }

        public void roationX(List<Body> body, int bodyIndex, List<Single> vertices, double angle, List<Single> Xlist, List<Single> Ylist, List<Single> Zlist,List<float> Centers)
        {
            if (body[bodyIndex].bTracked)
            {
                Centers.Clear();
                double radian = Math.PI * angle / 180.0;
                int verticesToread = 0;
                while (verticesToread < vertices.Count)
                {
                    vertices[verticesToread] = vertices[verticesToread];
                    vertices[verticesToread + 1] = (vertices[verticesToread + 1] * (float)Math.Cos(radian)) - (vertices[verticesToread + 2] * (float)Math.Sin(radian));
                    vertices[verticesToread + 2] = (vertices[verticesToread + 1] * (float)Math.Sin(radian)) + (vertices[verticesToread + 2] * (float)Math.Cos(radian));
                    Xlist.Add(vertices[verticesToread]);
                    Ylist.Add(vertices[verticesToread + 1]);
                    Zlist.Add(vertices[verticesToread + 2]);

                    verticesToread += 3;
                }

                Centers.Add(Center(Xlist));
                Centers.Add(Center(Ylist));
                Centers.Add(Center(Zlist));
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

       
        private void ResetHoloLens_Click(object sender, EventArgs e)
        {
            BodyViewIsOn = !BodyViewIsOn;
           // oTransferServer.Resetting = IsOn;//just added this 
        }

       

        private void Stop_Click(object sender, EventArgs e)
        {
            StopIsOn = !StopIsOn;
           
        }

        private void ScollDis_Click(object sender, EventArgs e)
        {
           
            similarRandomNumberIndex.Clear();
            ScollIsOn = !ScollIsOn;
            //groupBox1.Enabled = ScollIsOn;

            if (ScollIsOn == true)
            {
                Random random = new Random();

                var randomNumbers = Enumerable.Range(0, 9).OrderBy(x => random.Next()).Take(9).ToList();


                int i = 0;

                while (i < 9)
                {

                    similarRandomNumberIndex.Push(randomNumbers[i]);



                    i++;

                }

                int bodyIndex = similarRandomNumberIndex.Pop();
                size = sizelist[bodyIndex];
                idealText.Text = size.ToString();
                CounterIdeal.Text = -(similarRandomNumberIndex.Count - 9) + "";

                RowInScales.Add("Ideal Body");
                RowInAnswers.Add("IdealBodyAns");
                RowInScales.Add(size + "");
                // CheckPointSize();
            }
        }

        private void SimilarBody_Click(object sender, EventArgs e)
        {
           
            similarRandomNumberIndex.Clear();
            SimilarBodyIsOn = !SimilarBodyIsOn;
            

            if (SimilarBodyIsOn == true)
            {
                Random random = new Random();

                var randomNumbers = Enumerable.Range(0, 9).OrderBy(x => random.Next()).Take(9).ToList();
              

                int i = 0;

                while (i < 9)
                {

                    similarRandomNumberIndex.Push(randomNumbers[i]);

                    

                    i++;

                }

                int bodyIndex = similarRandomNumberIndex.Pop();
                size = sizelist[bodyIndex];
                sizeText.Text = size.ToString();
                CounterSimilar.Text = -(similarRandomNumberIndex.Count - 9) + "";
                RowInScales.Add("Similar Body");
                RowInAnswers.Add("SimilarBodyAns");
                RowInScales.Add(size + "");
                // CheckPointSize();
            }

        }

       
        

      

        private void RandomBodiesArray_Click(object sender, EventArgs e)
        {
            randomNumberIndex.Clear();
            randbuttonIsOn = !randbuttonIsOn;
            

            if (randbuttonIsOn == true)
            {
                Random random = new Random();

                var randomNumbers = Enumerable.Range(0, 9).OrderBy(x => random.Next()).Take(9).ToList();
                var randomNumbers2 = Enumerable.Range(0, 9).OrderBy(x => random.Next()).Take(9).ToList();
                var randomNumbers3 = Enumerable.Range(0, 9).OrderBy(x => random.Next()).Take(9).ToList();
                var randomNumbers4 = Enumerable.Range(0, 9).OrderBy(x => random.Next()).Take(9).ToList();
                var randomNumbers5 = Enumerable.Range(0, 9).OrderBy(x => random.Next()).Take(9).ToList();

                List<int> randomTemplist = new List<int>();
                randomTemplist.AddRange(randomNumbers);
                randomTemplist.AddRange(randomNumbers2);
                randomTemplist.AddRange(randomNumbers3);
                randomTemplist.AddRange(randomNumbers4);
                randomTemplist.AddRange(randomNumbers5);

                int i = 0;

                while (i < 45)
                {

                    randomNumberIndex.Push(randomTemplist[i]);


                    i++;

                }
                
                int bodyIndex = randomNumberIndex.Pop();
                size = sizelist[bodyIndex];
                Counter45.Text = -(randomNumberIndex.Count - 45) + "";
                RandomBodyText.Text = size.ToString();
                RowInScales.Add("Random45");
                RowInAnswers.Add("Answers45");
               
                // CheckPointSize();
            }

        }



        private void BodySizeYes_Click(object sender, EventArgs e)
        {
            if (StopIsOn == true)
            {
                return;
            }
            if(SimilarBodyIsOn == true)
            {
                return;
            }
            if(ScollIsOn == true)
            {
                return;
            }

            RowInAnswers.Add("1");
            RowInScales.Add(size + "");
                
            if (randomNumberIndex.Count != 0)
            {
                int bodyIndex = randomNumberIndex.Pop();
                size = sizelist[bodyIndex];
                RandomBodyText.Text = size.ToString();
                Counter45.Text = -(randomNumberIndex.Count - 45) + "";

            }
            else
            {
                Console.WriteLine("Array is Empty or size is 0");
                randbuttonIsOn = false;
            }

            StopIsOn = true;
        }


        private void BodySizeNo_Click(object sender, EventArgs e)
        {
            if(StopIsOn == true)
            {
                return;
            }
            if (SimilarBodyIsOn == true)
            {
                return;
            }
            if (ScollIsOn == true)
            {
                return;
            }

            RowInAnswers.Add("0");
            RowInScales.Add(size + "");

            if (randomNumberIndex.Count != 0)
            {
                int bodyIndex = randomNumberIndex.Pop();
                size = sizelist[bodyIndex];
                RandomBodyText.Text = size.ToString();
                Counter45.Text = -(randomNumberIndex.Count - 45) + "";

            }
            else
            {
                Console.WriteLine("Array is Empty or size is 0");
                randbuttonIsOn = false;
            }

            StopIsOn = true;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
           
            StopIsOn = false;
        }

        private void CheckPointSize()
        {
            Console.WriteLine("This is working");
            SizePoint.Clear();
            
           
            float pointSize = (size / 200f) + 0.003f;
            SizePoint.Add(pointSize);
            SizePoint.Add(pointSize);
            SizePoint.Add(pointSize);
            Console.WriteLine(Size + ": " + pointSize);
        }

        private void plusSimilar_Click(object sender, EventArgs e)
        {
            if (StopIsOn == true)
            {
                return;
            }
            if (randbuttonIsOn == true)
            {
                return;
            }
            if (ScollIsOn == true)
            {
                return;
            }
            if(size >= 1.4f)
            {
                return;
            }
            
            
            
            
            size += 0.1f;
            sizeText.Text = size.ToString();
        }

        private void minusSimilar_Click(object sender, EventArgs e)
        {
            if (StopIsOn == true)
            {
                return;
            }
            if (randbuttonIsOn == true)
            {
                return;
            }
            if (ScollIsOn == true)
            {
                return;
            }
            if (size <= 0.6f)
            {
                return;
            }
           

            size -= 0.1f;
            sizeText.Text = size.ToString();
        }

        private void SimilarYes_Click(object sender, EventArgs e)
        {
            if (StopIsOn == true)
            {
                return;
            }
            if (randbuttonIsOn == true)
            {
                return;
            }
            if (ScollIsOn == true)
            {
                return;
            }

            
            RowInAnswers.Add(size+"");


            if (similarRandomNumberIndex.Count != 0)
            {
                int bodyIndex = similarRandomNumberIndex.Pop();
                size = sizelist[bodyIndex];
                sizeText.Text = size.ToString();
                RowInScales.Add(size + "");
                CounterSimilar.Text = -(similarRandomNumberIndex.Count - 9) + "";



            }
            else
            {
                Console.WriteLine("Array is Empty or size is 0");
                SimilarBodyIsOn = false;
            }

            StopIsOn = true;
        }

        private void SimilarNo_Click(object sender, EventArgs e)
        {
            if (StopIsOn == true)
            {
                return;
            }
            if (randbuttonIsOn == true)
            {
                return;
            }
            if (ScollIsOn == true)
            {
                return;
            }

            RowInAnswers.Add("0");
           


            if (similarRandomNumberIndex.Count != 0)
            {
                int bodyIndex = similarRandomNumberIndex.Pop();
                size = sizelist[bodyIndex];
                sizeText.Text = size.ToString();
                RowInScales.Add(size + "");
                CounterSimilar.Text = -(similarRandomNumberIndex.Count - 9) + "";
               



            }
            else
            {
                Console.WriteLine("Array is Empty or size is 0");
                SimilarBodyIsOn = false;
            }

            StopIsOn = true;
        }

        private void similarNext_Click(object sender, EventArgs e)
        {
            StopIsOn = false;
        }

        private void IdealPlus_Click(object sender, EventArgs e)
        {
            if (StopIsOn == true)
            {
                return;
            }
            if (randbuttonIsOn == true)
            {
                return;
            }
            if (SimilarBodyIsOn == true)
            {
                return;
            }
            if (size >= 1.4f)
            {
                return;
            }


            size += 0.1f;
            idealText.Text = size.ToString();
        }

        private void IdeaMinus_Click(object sender, EventArgs e)
        {
            if (StopIsOn == true)
            {
                return;
            }
            if (randbuttonIsOn == true)
            {
                return;
            }
            if (SimilarBodyIsOn == true)
            {
                return;
            }
            if (size <= 0.6f)
            {
                return;
            }


            size -= 0.1f;
            idealText.Text = size.ToString();
        }

        private void IdeaYes_Click(object sender, EventArgs e)
        {
            if (StopIsOn == true)
            {
                return;
            }
            if (randbuttonIsOn == true)
            {
                return;
            }
            if (SimilarBodyIsOn == true)
            {
                return;
            }

            RowInAnswers.Add(size + "");
           


            if (similarRandomNumberIndex.Count != 0)
            {
                int bodyIndex = similarRandomNumberIndex.Pop();
                size = sizelist[bodyIndex];
                idealText.Text = size.ToString();
                RowInScales.Add(size + "");
                CounterIdeal.Text = -(similarRandomNumberIndex.Count - 9) + "";



            }
            else
            {
                Console.WriteLine("Array is Empty or size is 0");
                ScollIsOn = false;
            }

            StopIsOn = true;
        }

        private void IdealNo_Click(object sender, EventArgs e)
        {
            if (StopIsOn == true)
            {
                return;
            }
            if (randbuttonIsOn == true)
            {
                return;
            }
            if (SimilarBodyIsOn == true)
            {
                return;
            }

            RowInAnswers.Add("0");
           


            if (similarRandomNumberIndex.Count != 0)
            {
                int bodyIndex = similarRandomNumberIndex.Pop();
                size = sizelist[bodyIndex];
                idealText.Text = size.ToString();
                RowInScales.Add(size + "");
                CounterIdeal.Text = -(similarRandomNumberIndex.Count - 9) + "";



            }
            else
            {
                Console.WriteLine("Array is Empty or size is 0");
                ScollIsOn  = false;
            }

            StopIsOn = true;
        }

        private void IdealNext_Click(object sender, EventArgs e)
        {
            StopIsOn = false;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            RowInScales.Clear();
            randomNumberIndex.Clear();
            similarRandomNumberIndex.Clear();
            ScollIsOn = false;
            randbuttonIsOn = false;
            SimilarBodyIsOn = false;
          
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (csv_scales != null)
            {
                csv_scales.WriteRow(RowInScales);
                csv_answers.WriteRow(RowInAnswers);
                

                RowInScales.Clear();
                RowInAnswers.Clear();
                randomNumberIndex.Clear();
                similarRandomNumberIndex.Clear();
                ScollIsOn = false;
                randbuttonIsOn = false;
                SimilarBodyIsOn = false;
            }
        }

        private void New_Click(object sender, EventArgs e)
        {
            RowInScales.Clear();
            RowInAnswers.Clear();
            randomNumberIndex.Clear();
            similarRandomNumberIndex.Clear();
            ScollIsOn = false;
            randbuttonIsOn = false;
            SimilarBodyIsOn = false;
            
            if(ParticipantsID.Text != null)
            {
               
                RowInAnswers.Add(ParticipantsID.Text);
                RowInScales.Add(ParticipantsID.Text); 
            }
           
                      
        }

        private void AddFile_Click(object sender, EventArgs e)
        {
            string filePath = "C:\\Users\\nimch681\\Documents\\LiveScan3D\\LiveScan3D-master";

          
                String scalesFileString = filePath + "/Holo_scales.csv";
                csv_scales = new CsvFileWriter(scalesFileString);
                String answersFileString = filePath + "/Holo_answers.csv";
                csv_answers = new CsvFileWriter(answersFileString);
              
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (csv_scales != null)
            {
                csv_scales.Dispose();
                csv_answers.Dispose();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
