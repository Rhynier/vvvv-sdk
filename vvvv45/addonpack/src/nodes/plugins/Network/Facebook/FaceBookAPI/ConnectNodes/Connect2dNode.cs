using System;
using System.Collections.Generic;
using System.Text;
using VVVV.PluginInterfaces.V1;
using Facebook.API;
using vvvv.Nodes.Internal;
using FaceBookAPI.Internal;

namespace vvvv.Nodes
{
    public class Connect2dNode : IPlugin
    {
        #region Plugin Info
        public static IPluginInfo PluginInfo
        {
            get
            {
                IPluginInfo Info = new PluginInfo();
                Info.Name = "Connect2d";							//use CamelCaps and no spaces
                Info.Category = "Facebook";						//try to use an existing one
                Info.Version = "";						//versions are optional. leave blank if not needed
                Info.Help = "Connects List of friends within facebook";
                Info.Bugs = "";
                Info.Credits = "";
                Info.Warnings = "";

                //leave below as is
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                System.Diagnostics.StackFrame sf = st.GetFrame(0);
                System.Reflection.MethodBase method = sf.GetMethod();
                Info.Namespace = method.DeclaringType.Namespace;
                Info.Class = method.DeclaringType.Name;
                return Info;
                //leave above as is
            }
        }
        #endregion

        private IPluginHost FHost;

        private IStringIn FPinInUsers;

        private IValueIn FPinInX;
        private IValueIn FPinInY;

        private IValueOut FPinOutX1;
        private IValueOut FPinOutX2;
        private IValueOut FPinOutY1;
        private IValueOut FPinOutY2;

        private List<string> FUsers = new List<string>();
        private UserLinkList FLinks = new UserLinkList();

        private FacebookAPI api;

        #region Set Plugin Host
        public void SetPluginHost(IPluginHost Host)
        {
            this.FHost = Host;

            this.FHost.CreateStringInput("Users", TSliceMode.Dynamic, TPinVisibility.True, out this.FPinInUsers);

            //Input
            this.FHost.CreateValueInput("X In", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPinInX);
            this.FPinInX.SetSubType(double.MinValue, double.MaxValue, 0.01, 0, false, false, false);

            this.FHost.CreateValueInput("Y In", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPinInY);
            this.FPinInY.SetSubType(double.MinValue, double.MaxValue, 0.01, 0, false, false, false);

            //Output
            this.FHost.CreateValueOutput("X1 Out", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPinOutX1);
            this.FPinOutX1.SetSubType(double.MinValue, double.MaxValue, 0.01, 0, false, false, false);

            this.FHost.CreateValueOutput("Y1 Out", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPinOutY1);
            this.FPinOutY1.SetSubType(double.MinValue, double.MaxValue, 0.01, 0, false, false, false);

            this.FHost.CreateValueOutput("X2 Out", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPinOutX2);
            this.FPinOutX2.SetSubType(double.MinValue, double.MaxValue, 0.01, 0, false, false, false);

            this.FHost.CreateValueOutput("Y2 Out", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPinOutY2);
            this.FPinOutY2.SetSubType(double.MinValue, double.MaxValue, 0.01, 0, false, false, false);

            this.api = APIShared.API;
        }
        #endregion

        #region Configurate
        public void Configurate(IPluginConfig Input)
        {

        }
        #endregion

        #region Evaluate
        public void Evaluate(int SpreadMax)
        {
            bool userchanged = false;

            #region Build Links
            if (this.FPinInUsers.PinIsChanged)
            {
                this.FUsers.Clear();
                this.FLinks.Clear();

                try
                {
                    for (int i = 0; i < this.FPinInUsers.SliceCount; i++)
                    {
                        string user;
                        this.FPinInUsers.GetString(i, out user);
                        for (int j = 0; j < this.FUsers.Count; j++)
                        {
                            //Need to update the query, otherwise, sends a Max request limit
                            if (api.AreFriends(user, this.FUsers[j]))
                            {
                                UserLink link = new UserLink();
                                link.User1 = user;
                                link.User2 = this.FUsers[j];
                                link.Index1 = i;
                                link.Index2 = j;

                                this.FLinks.Add(link);
                            }
                        }
                        this.FUsers.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error");
                }
                userchanged = true;
            }
            #endregion

            if (userchanged || this.FPinInX.PinIsChanged || this.FPinInY.PinIsChanged)
            {
                List<double> lstx = new List<double>();
                List<double> lsty = new List<double>();

                int idx = 0;
                int idy = 0;

                #region Build Points
                for (int i = 0; i < this.FPinInUsers.SliceCount; i++)
                {
                    double d;

                    this.FPinInX.GetValue(idx, out d);
                    lstx.Add(d);

                    this.FPinInY.GetValue(idy, out d);
                    lsty.Add(d);

                    idx++;
                    idy++;

                    if (idx >= this.FPinInX.SliceCount)
                    {
                        idx = 0;
                    }

                    if (idy >= this.FPinInY.SliceCount)
                    {
                        idy = 0;
                    }
                }
                #endregion

                this.FPinOutX1.SliceCount = this.FLinks.Count;
                this.FPinOutX2.SliceCount = this.FLinks.Count;
                this.FPinOutY1.SliceCount = this.FLinks.Count;
                this.FPinOutY2.SliceCount = this.FLinks.Count;

                for (int i = 0; i < this.FLinks.Count; i++)
                {   
                    this.FPinOutX1.SetValue(i, lstx[this.FLinks[i].Index1]);
                    this.FPinOutX2.SetValue(i, lstx[this.FLinks[i].Index2]);
                    this.FPinOutY1.SetValue(i, lsty[this.FLinks[i].Index1]);
                    this.FPinOutY2.SetValue(i, lsty[this.FLinks[i].Index2]);
                }
            }
        }
        #endregion

        #region Auto Evaluate
        public bool AutoEvaluate
        {
            get { return false; }
        }
        #endregion
    }
}
