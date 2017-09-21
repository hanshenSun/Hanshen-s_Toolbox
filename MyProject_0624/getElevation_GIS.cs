using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class getElevation_GIS : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the getElevation_GIS class.
        /// </summary>
        public getElevation_GIS()
          : base("getElevation_GIS", "getElevation",
              "Get Elevation data in meters using Google Elevation API",
              "HS_ToolBox", "GIS")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Lat", "Latitude", "Latitude", GH_ParamAccess.list);
            pManager.AddNumberParameter("Lon", "Longitude", "Longitude", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Res", "Resolution", "Resolution", GH_ParamAccess.item);
            pManager[2].Optional = true;
            //pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Lat", "Lat", "Queryed Latitude in meters", GH_ParamAccess.list);
            pManager.AddNumberParameter("Lon", "Lon", "Queryed Longitude in meters", GH_ParamAccess.list);
            pManager.AddNumberParameter("Elevation", "Ele", "Queryed Elevation in meters", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            /*
            string inputLatitudeA = "36.578581";
            string inputLatitudeB = "-118.291994";
            string inputLongitudeA = "36.23998";
            string inputLongitudeB = "-116.83171";
            */


            List<double> inputLatitude = new List<double>();
            List<double> inputLongitude = new List<double>();

            int resCount = 3;


            DA.GetDataList(0, inputLatitude);
            DA.GetDataList(1,  inputLongitude);
            DA.GetData(2, ref resCount);

            int countA = inputLongitude.Count;
            int coutB = inputLatitude.Count;

            string inputLatitudeA = "36.578581";
            string inputLatitudeB = "-118.291994";
            string inputLongitudeA = "36.23998";
            string inputLongitudeB = "-116.83171";

            inputLatitudeA = inputLatitude[0].ToString();
            inputLatitudeB = inputLatitude[1].ToString();
            


            inputLongitudeA = inputLongitude[0].ToString();
            inputLongitudeB = inputLongitude[1].ToString();


            string ELEVATION_BASE_URL = "https://maps.googleapis.com/maps/api/elevation/json?path=";
            string path = inputLatitudeA + "," + inputLongitudeA +   " | " + inputLatitudeB + "," + inputLongitudeB;
            string actualUrl = ELEVATION_BASE_URL + path + "&samples=" + resCount;


            //response = simplejson.load(urllib.urlopen(url))

            WebClient wc = new WebClient();

            var js = wc.DownloadString(actualUrl);
            JObject json = JObject.Parse(js);


            List<double> latData = new List<double>();
            List<double> lonData = new List<double>();
            List<double> elevationData = new List<double>();

            

            
            foreach (var resultset in json["results"])
            {
                string elevationStr = resultset["elevation"].ToString();
                double elevationDouble = Convert.ToDouble(elevationStr);

                elevationData.Add(elevationDouble);

                //var lonStrr = resultset["location"].ToString();

                string latStr = resultset["location"].First.ToString().Split(':')[1];
                double latDouble = Convert.ToDouble(latStr);
                latData.Add(latDouble);


                string lonStr = resultset["location"].Last.ToString().Split(':')[1];
                double lonDouble = Convert.ToDouble(lonStr);
                lonData.Add(lonDouble);
                /*
                foreach (var locationset in resultset["location"])
                {



                }
                */

            }
            

            DA.SetDataList(0, latData);
            DA.SetDataList(1, lonData);
            DA.SetDataList(2, elevationData);
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
            get { return new Guid("aa400a42-54f7-4943-811f-83d754529fdf"); }
        }
    }
}