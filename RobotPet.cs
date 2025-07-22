// Note: Add references to AForge.Video, AForge.Video.DirectShow, and Emgu.CV (install via NuGet for AForge.NET and Emgu.CV)
using System;
using System.Drawing;
using AForge.Video;
using AForge.Video.DirectShow;
using RoboDK;
using Emgu.CV;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace CompanionBot
{
    class Program
    {
        static VideoCaptureDevice videoDevice;
        static CascadeClassifier faceDetector;
        static Net emotionModel;
        static string[] emotions = { "angry", "disgust", "fear", "happy", "sad", "surprise", "neutral" };

        static void Main(string[] args)
        {
            // Initialize face detection
            faceDetector = new CascadeClassifier("haarcascade_frontalface_default.xml"); // Download Haar cascade file

            // Initialize emotion model (assumes a pretrained ONNX model, needs to be downloaded)
            emotionModel = DnnInvoke.ReadNetFromONNX("emotion-ferplus.onnx"); // Example model file
            emotionModel.SetPreferableTarget(Target.Cpu);

            // Initialize camera
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                Console.WriteLine("No camera device found."); 
                return;
            }
            videoDevice = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoDevice.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoDevice.Start();

            // Initialize the RoboDK API
            var rdk = new RDK();
            if (!rdk.Connected())
            {
                Console.WriteLine("Unable to connect to RoboDK. Please ensure RoboDK is running.");
                return;
            }

            // Load the robot (assumed configured in RoboDK)
            var robot = rdk.Item("CompanionRobot", ITEM_TYPE_ROBOT);
            if (robot.Valid())
            {
                Console.WriteLine("Robot loaded successfully.");

                // Conversation loop
                while (true)
                {
                    Console.WriteLine("Please enter your message (type 'exit' to quit):");
                    string userInput = Console.ReadLine();
                    if (userInput.ToLower() == "exit")
                        break;

                    // Simulate sensor input (e.g., user emotion); simple text-based determination here
                    string userEmotion = DetectEmotion(userInput);

                    // Simple AI core example: respond based on detected emotion
                    string responseAction = GetResponseAction(userEmotion);

                    // Generate dialogue response
                    string responseText = GenerateResponse(userEmotion);
                    Console.WriteLine($"Robot response: {responseText}");

                    // 执行机器人动作
                    ExecuteRobotAction(robot, responseAction);
                }
            }
            else
            {
                Console.WriteLine("Robot item not found.");
            }

            // Stop camera
            videoDevice.Stop();
        }

        // New frame event handler: emotion detection with Emgu.CV
        static void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            using (Mat frame = CvInvoke.Imread(eventArgs.Frame, ImreadModes.Color));
            using (Mat gray = new Mat());
            CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
            Rectangle[] faces = faceDetector.DetectMultiScale(gray, 1.1, 5);

            if (faces.Length > 0)
            {
                // Take the first detected face
                Rectangle face = faces[0];
                using (Mat faceMat = new Mat(gray, face));
                CvInvoke.Resize(faceMat, faceMat, new Size(64, 64)); // Assuming model input size

                // Preprocess and predict
                using (Mat blob = DnnInvoke.BlobFromImage(faceMat, 1.0 / 255, new Size(64, 64), new MCvScalar(), false, false));
                emotionModel.SetInput(blob);
                Mat output = emotionModel.Forward();

                // Get emotion with the highest score
                float[] scores = output.GetData(true) as float[];
                int maxIndex = Array.IndexOf(scores, scores.Max());
                string detectedEmotion = emotions[maxIndex];

                // Here you can update a global emotion variable or trigger a response
                Console.WriteLine($"Detected emotion: {detectedEmotion}");
                // Example: userEmotion = detectedEmotion; // Integrate into dialogue if needed
            }
        }

        // Simple emotion detection (text-based fallback)
        static string DetectEmotion(string input)
        {
            if (input.Contains("开心") || input.Contains("happy"))
                return "happy";
            if (input.Contains("伤心") || input.Contains("sad"))
                return "sad";
            return "neutral";
        }

        // Generate text response
        static string GenerateResponse(string emotion)
        {
            return emotion switch
            {
                "happy" => "Great! I'm happy for you too.",
                "sad" => "Don't be sad, I'm here with you.",
                _ => "I'm listening."
            };
        }

        // Simple AI algorithm: emulate LOVOT/Ropet-style emotional response
        static string GetResponseAction(string emotion)
        {
            return emotion switch
            {
                "happy" => "wave",
                "sad" => "hug",
                _ => "idle"
            };
        }

        // 执行机器人动作
        static void ExecuteRobotAction(Item robot, string action)
        {
            switch (action)
            {
                case "wave":
                    // Simple joint movement: wave
                    robot.MoveJ(new double[] { 0, -90, 90, 0, 90, 0 });
                    break;
                case "hug":
                    // Simulate hug action
                    robot.MoveJ(new double[] { 0, -45, 45, 0, 45, 0 });
                    break;
                default:
                    Console.WriteLine("No action.");
                    break;
            }
            Console.WriteLine($"Executing action: {action}");
        }
    }
}