using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;


namespace MyProject_0624
{
    public class imageReader : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the imageReader class.
        /// </summary>
        public imageReader()
          : base("imageReader", "imageReader",
              "imageReader",
              "HS_ToolBox", "img")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("p", "path", "path", GH_ParamAccess.item);
            pManager.AddBooleanParameter("get color?", "get color?", "get color?", GH_ParamAccess.item);
            pManager[1].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("width", "width", "width", GH_ParamAccess.item);
            pManager.AddNumberParameter("height", "height", "height", GH_ParamAccess.item);
            pManager.AddTextParameter("color", "color", "color", GH_ParamAccess.tree);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string filePath = string.Empty;
            bool runBool = false;

            DA.GetData(0, ref filePath);
            DA.GetData(1, ref runBool);

            //string address = "@"+filePath;
            System.Drawing.Image img = System.Drawing.Image.FromFile(filePath);
            Bitmap imgBitmap = new Bitmap(img);
            int imgWidth = imgBitmap.Width;
            int imgHeight = imgBitmap.Height;
            //List<List<string>> outputData = new List<List<string>>();
            Grasshopper.DataTree<string> outputTree = new Grasshopper.DataTree<string>();

            if (runBool)
            {
                for( int x = 0; x < imgWidth; x++)
                {
                    //List<string> xdata = new List<string>();
                    Grasshopper.Kernel.Data.GH_Path treePath = new Grasshopper.Kernel.Data.GH_Path(x);
                    for (int y = 0; y < imgHeight; y++)
                    {
                        Color pixcelColor = imgBitmap.GetPixel(x, y);
                        outputTree.Add(pixcelColor.R.ToString() + "," +pixcelColor.G.ToString() + "," + pixcelColor.B.ToString(), treePath);
                    }
                }
            }
            DA.SetData(0, imgWidth);
            DA.SetData(1, imgHeight);
            DA.SetDataTree(2, outputTree);
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
            get { return new Guid("7f51c023-35c2-4466-a865-7350648be1a6"); }
        }
    }
}