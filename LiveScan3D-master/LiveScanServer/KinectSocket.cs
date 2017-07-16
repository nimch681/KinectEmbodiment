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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace KinectServer
{
    public delegate void SocketChangedHandler();

    public class KinectSocket
    {
        Socket oSocket;
        byte[] byteToSend = new byte[1];
        public bool bFrameCaptured = false;
        public bool bLatestFrameReceived = false;
        public bool bStoredFrameReceived = false;
        public bool bNoMoreStoredFrames = true;
        public bool bCalibrated = false;
        //The pose of the sensor in the scene (used by the OpenGLWindow to show the sensor)
        public AffineTransform oCameraPose = new AffineTransform();
        //The transform that maps the vertices in the sensor coordinate system to the world corrdinate system.
        public AffineTransform oWorldTransform = new AffineTransform();

        public string sSocketState;

        public List<byte> lFrameRGB = new List<byte>();
        public List<Single> lFrameVerts = new List<Single>();
        public List<Body> lBodies = new List<Body>();

        public event SocketChangedHandler eChanged;

        

        public KinectSocket(Socket clientSocket)
        {
            oSocket = clientSocket;
            sSocketState = oSocket.RemoteEndPoint.ToString() + " Calibrated = false";
        }

        public void CaptureFrame()
        {
            bFrameCaptured = false;
            byteToSend[0] = 0;
            SendByte();
        }

        public void Calibrate()
        {
            bCalibrated = false;
            sSocketState = oSocket.RemoteEndPoint.ToString() + " Calibrated = false";

            byteToSend[0] = 1;
            SendByte();

            UpdateSocketState();
        }

        public void SendSettings(KinectSettings settings)
        {
            List<byte> lData = settings.ToByteList();

            byte[] bTemp = BitConverter.GetBytes(lData.Count);
            lData.InsertRange(0, bTemp);
            lData.Insert(0, 2);

            if (SocketConnected())
                oSocket.Send(lData.ToArray());
        }

        public void RequestStoredFrame()
        {
            byteToSend[0] = 3;
            SendByte();
            bNoMoreStoredFrames = false;
            bStoredFrameReceived = false;
        }

        public void RequestLastFrame()
        {
            byteToSend[0] = 4;
            SendByte();
            bLatestFrameReceived = false;
        }

        public void SendCalibrationData()
        {
            int size = 1 + (9 + 3) * sizeof(float);
            byte[] data = new byte[size];
            int i = 0;

            data[i] = 5;
            i++;

            Buffer.BlockCopy(oWorldTransform.R, 0, data, i, 9 * sizeof(float));
            i += 9 * sizeof(float);
            Buffer.BlockCopy(oWorldTransform.t, 0, data, i, 3 * sizeof(float));
            i += 3 * sizeof(float);

            if (SocketConnected())
                oSocket.Send(data);
        }

        public void ClearStoredFrames()
        {
            byteToSend[0] = 6;
            SendByte();
        }

        public void ReceiveCalibrationData()
        {
            bCalibrated = true;

            byte[] buffer = Receive(sizeof(int) * 1);
            //currently not used
            int markerId = BitConverter.ToInt32(buffer, 0);

            buffer = Receive(sizeof(float) * 9);
            Buffer.BlockCopy(buffer, 0, oWorldTransform.R, 0, sizeof(float) * 9);

            buffer = Receive(sizeof(float) * 3);
            Buffer.BlockCopy(buffer, 0, oWorldTransform.t, 0, sizeof(float) * 3);

            oCameraPose.R = oWorldTransform.R;
            for (int i = 0; i < 3; i++)
            {
                oCameraPose.t[i] = 0.0f;
                for (int j = 0; j < 3; j++)
                {
                    oCameraPose.t[i] += oWorldTransform.t[j] * oWorldTransform.R[i, j];
                }
            }

            UpdateSocketState();
        }

        public void ReceiveFrame()
        {

            lFrameRGB.Clear();
            lFrameVerts.Clear();
            lBodies.Clear();

            int nToRead;
            byte[] buffer = new byte[1024];

            while (oSocket.Available == 0)
            {
                if (!SocketConnected())
                    return;
            }

            oSocket.Receive(buffer, 8, SocketFlags.None);
            nToRead = BitConverter.ToInt32(buffer, 0);
            int iCompressed = BitConverter.ToInt32(buffer, 4);

            if (nToRead == -1)
            {
                bNoMoreStoredFrames = true;
                return;
            }

            buffer = new byte[nToRead];
            int nAlreadyRead = 0;

            while (nAlreadyRead != nToRead)
            {
                while (oSocket.Available == 0)
                {
                    if (!SocketConnected())
                        return;
                }

                nAlreadyRead += oSocket.Receive(buffer, nAlreadyRead, nToRead - nAlreadyRead, SocketFlags.None);
            }




            if (iCompressed == 1)
                buffer = ZSTDDecompressor.Decompress(buffer);

            //Receive depth and color data
            int startIdx = 0;

            int n_vertices = BitConverter.ToInt32(buffer, startIdx);
            startIdx += 4;
            List<Single> TempVerts = new List<Single>();
            List<byte> TempRGB = new List<byte>();
            for (int i = 0; i < n_vertices; i++)
            {


                for (int j = 0; j < 3; j++)
                {

                    TempRGB.Add(buffer[startIdx++]);
                }
                for (int j = 0; j < 3; j++)
                {
                    float val = BitConverter.ToInt16(buffer, startIdx);
                    //converting from milimeters to meters
                    val /= 1000.0f;
                    TempVerts.Add(val);

                    startIdx += 2;
                }



            }

            //Receive body data
            int nBodies = BitConverter.ToInt32(buffer, startIdx);
            startIdx += 4;
            for (int i = 0; i < nBodies; i++)
            {
                Body tempBody = new Body();
                tempBody.bTracked = BitConverter.ToBoolean(buffer, startIdx++);
                int nJoints = BitConverter.ToInt32(buffer, startIdx);
                startIdx += 4;

                tempBody.lJoints = new List<Joint>(nJoints);
                tempBody.lJointsInColorSpace = new List<Point2f>(nJoints);

                for (int j = 0; j < nJoints; j++)
                {
                    Joint tempJoint = new Joint();
                    Point2f tempPoint = new Point2f();

                    tempJoint.jointType = (JointType)BitConverter.ToInt32(buffer, startIdx);
                    startIdx += 4;
                    tempJoint.trackingState = (TrackingState)BitConverter.ToInt32(buffer, startIdx);
                    startIdx += 4;
                    tempJoint.position.X = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;
                    tempJoint.position.Y = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;
                    tempJoint.position.Z = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;

                    tempPoint.X = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;
                    tempPoint.Y = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;

                    tempBody.lJoints.Add(tempJoint);
                    tempBody.lJointsInColorSpace.Add(tempPoint);
                }

                lBodies.Add(tempBody);



            }



            /**body segmentation starts here*/


            List<Single> HeadVert = new List<Single>();
            List<byte> HeadRGB = new List<byte>();
            List<Single> RighHandtVert = new List<Single>();
            List<byte> RightHandRGB = new List<byte>();
            List<Single> LeftLegVert = new List<Single>();
            List<byte> LeftLegRGB = new List<byte>();
            List<Single> LowerabdomenVert = new List<Single>();
            List<byte> LowerabdomenRGB = new List<byte>();

            List<Single> UpperabdomenVert = new List<Single>();
            List<byte> UpperabdomenRGB = new List<byte>();

            List<Single> HeadX = new List<Single>();
            List<Single> HeadY = new List<Single>();
            List<Single> HeadZ = new List<Single>();

            List<Single> LowerabdomenX = new List<Single>();
            List<Single> LowerabdomenY = new List<Single>();
            List<Single> LowerabdomenZ = new List<Single>();


            List<Single> UpperabdomenX = new List<Single>();
            List<Single> UpperabdomenY = new List<Single>();
            List<Single> UpperabdomenZ = new List<Single>();

            for (int i = 0; i < 6; i++)
            {

                if (lBodies[i].bTracked)
                {
                   

                    int pdidx = 0;
                    int nAllVertices = n_vertices*3;
                    while (pdidx < nAllVertices)
                    {
                        BodySegmentWithVerticesArray(HeadVert, HeadRGB,lBodies, i, 3, TempVerts, TempRGB, pdidx, 0.15f, 0.15f, 0.10f, 0.15f, 0.15f, 0.15f, HeadX, HeadY, HeadZ );
                        BodySegment(RighHandtVert, RightHandRGB,lBodies, i, 11, TempVerts, TempRGB, pdidx, 0.10f, 0.10f, 0.10f, 0.10f, 0.05f, 0.05f);

                        
                       

                        float ShoulderY = lBodies[i].lJoints[20].position.Y;
                       
                        
                       // BodySegmentWithAbsoluteMaxY(UpperabdomenVert, UpperabdomenRGB, lBodies, i, 1, TempVerts, TempRGB, pdidx, 0.20f, 0.20f, 0f, ShoulderY, 0.10f, 0.10f, UpperabdomenX, UpperabdomenY, UpperabdomenZ);

                        BodySegmentWithAbsoluteMaxY(LowerabdomenVert, LowerabdomenRGB, lBodies, i, 0, TempVerts, TempRGB, pdidx, 0.20f, 0.20f, 0f, ShoulderY, 0.20f, 0.20f, LowerabdomenX, LowerabdomenY, LowerabdomenZ);

                        // BodySegment(SpineMidVert, SpineMidRGB, lBodies, i, 19, TempVerts, TempRGB, pdidx, 0.20f, 0.20f, SpineBaseY - , , 0.30f, 0.30f);
                        pdidx += 3;

                    }

                   


                    //JointType jointType = lBodies[i].lJoints[3].jointType;
                    //int JT = (int)jointType;


                    //Console.Write("Body " + i + " is tracked\n" +
                    // "Position (x, y, z): " + headX + ", " + headY + ", " + headZ + "\n" +
                    // "Joint: " + JT + "\n");
                }

                scale(lBodies, i ,HeadVert, 1.5f, 1f, 1.5f, HeadX, HeadY, HeadZ);
               // scale(lBodies, i, UpperabdomenVert, 1.5f, 1f, 1.5f, UpperabdomenX, UpperabdomenY, UpperabdomenZ);
                scale(lBodies, i, LowerabdomenVert, 1.5f, 1f, 1.5f, LowerabdomenX, LowerabdomenY, LowerabdomenZ);
            }





            // Console.Write("Tempdvert size : " + TempVerts.Count + "\n");
            //Console.Write("TempRGB size : " + TempRGB.Count + "\n");

            //scale(HeadVert, 1.5f, 2f, 2f, HeadX, HeadY, HeadZ);


            // lFrameVerts.AddRange(RighHandtVert);
            //lFrameRGB.AddRange(RightHandRGB);
            lFrameVerts.AddRange(LowerabdomenVert);
            lFrameRGB.AddRange(LowerabdomenRGB);

            //lFrameVerts.AddRange(UpperabdomenVert);
           // lFrameRGB.AddRange(UpperabdomenRGB);

            lFrameVerts.AddRange(HeadVert);
            lFrameRGB.AddRange(HeadRGB);
            
            //    //        //float x = lBodies[i].lJoints[3].position.X;

            //    //        //JointType jointType = lBodies[i].lJoints[3].jointType;
            //    //        //int JT = (int)jointType;

            //    //        //Console.Write("Body " + i + " is not tracked " + " Position " + x);

            //    //        // float y  ....



            //    //}
            //}

        }

        float DistanceN(float[] first, float[] second)
        {
            var sum = first.Select((x, i) => (x - second[i]) * (x - second[i])).Sum();
            return (float)Math.Sqrt(sum);
        }

        public byte[] Receive(int nBytes)
        {
            byte[] buffer;
            if (oSocket.Available != 0)
            {
                buffer = new byte[Math.Min(nBytes, oSocket.Available)];
                oSocket.Receive(buffer, nBytes, SocketFlags.None);
            }
            else
                buffer = new byte[0];

            return buffer;
        }

        public bool SocketConnected()
        {
            bool part1 = oSocket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (oSocket.Available == 0);

            if (part1 && part2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void SendByte()
        {
            oSocket.Send(byteToSend);
        }

        public void UpdateSocketState()
        {
            if (bCalibrated)
                sSocketState = oSocket.RemoteEndPoint.ToString() + " Calibrated = true";
            else
                sSocketState = oSocket.RemoteEndPoint.ToString() + " Calibrated = false";

            if (eChanged != null)
                eChanged();
        }

        

        public void BodySegmentWithVerticesArray(List<Single> BodyPartVert, List<byte> BodyPartRGB, List<Body> body, int bodyIndex, int JointIndex,
            List<Single> vertArray, List<byte> RGBArray, int vertIndex, float mixX, float maxX, float minY, float maxY, float minZ, float maxZ, List<Single> Xlist, List<Single> Ylist, List<Single> Zlist)
        {
            

            if (body[bodyIndex].bTracked)
            {
                //Console.Write("This is working bodyIndex " + bodyIndex);
                float jointX = lBodies[bodyIndex].lJoints[JointIndex].position.X;
                float jointY = lBodies[bodyIndex].lJoints[JointIndex].position.Y;
                float jointZ = lBodies[bodyIndex].lJoints[JointIndex].position.Z;

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
                float jointX = lBodies[bodyIndex].lJoints[JointIndex].position.X;
                float jointY = lBodies[bodyIndex].lJoints[JointIndex].position.Y;
                float jointZ = lBodies[bodyIndex].lJoints[JointIndex].position.Z;

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
                float jointX = lBodies[bodyIndex].lJoints[JointIndex].position.X;
                float jointY = lBodies[bodyIndex].lJoints[JointIndex].position.Y;
                float jointZ = lBodies[bodyIndex].lJoints[JointIndex].position.Z;

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
                float jointX = lBodies[bodyIndex].lJoints[JointIndex].position.X;
                float jointY = lBodies[bodyIndex].lJoints[JointIndex].position.Y;
                float jointZ = lBodies[bodyIndex].lJoints[JointIndex].position.Z;

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

                Console.Write("The center is " + " X: " + centerX + " Y: " + centerY + " Z: " + centerZ + " \n");
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

    }
}
