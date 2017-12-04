using System;
using System.Collections.Generic;

using NAudio.Wave;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624._Audio
{
    public class GetAudioLength : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GetAudioLength()
          : base("getAudioLength", "getAudioLength",
              "get the length of the audio, in miliseconds",
              "HS_ToolBox", "Audio Player")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("length", "length", "Audio Length of the mp3", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            Mp3FileReader reader = new Mp3FileReader("<YourMP3>.mp3");
            TimeSpan duration = reader.TotalTime;
            //reader.

            double audioLength = 0.0;
            audioLength = duration.TotalMilliseconds;
            DA.SetData(0, audioLength);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bab59a01-0a44-467e-a7b3-b64c54739ab8"); }
        }
    }
}