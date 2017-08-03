using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace MyProject_0624
{
    public class HS_ToolboxInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "HS_Toolbox";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                
                return Properties.Resources.HS_ToolBox;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "This a set of functional tools aim to facilitate the daily workflow of BIM modellers, developped by Hanshen Sun who worked for Thornton Tomasetti";
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
                return "Hanshen Sun";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "email to hanshensunw@gmail.com for comments and suggestions";
            }
        }
    }
}
