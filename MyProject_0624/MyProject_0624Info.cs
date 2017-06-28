using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace MyProject_0624
{
    public class MyProject_0624Info : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "MyProject0624";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("609fa026-186a-4f41-9f5a-28a1f1c5cf67");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
