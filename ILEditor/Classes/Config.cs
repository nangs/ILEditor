﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEditor.Classes
{
    class Config
    {
        private string ConfigLocation;
        private Dictionary<string, string> Data;

        private void CheckExist(string key, string value)
        {
            if (!Data.ContainsKey(key))
                Data.Add(key, value);
        }

        public Config(string Location)
        {
            ConfigLocation = Location;
            Data = new Dictionary<string, string>();
            LoadConfig();
        }

        public void LoadConfig()
        {
            string[] data;
            if (File.Exists(ConfigLocation))
            {
                foreach (string Line in File.ReadAllLines(ConfigLocation))
                {
                    data = Line.Split('=');
                    for (int i = 0; i < data.Length; i++) data[i] = data[i].Trim();

                    if (Data.ContainsKey(data[0]))
                    {
                        Data[data[0]] = data[1];
                    }
                    else
                    {
                        Data.Add(data[0], data[1]);
                    }
                }
            }

            CheckExist("system", "system");
            CheckExist("username", "myuser");
            CheckExist("password", "mypass");

            CheckExist("datalibl", "SYSTOOLS");
            CheckExist("curlib", "SYSTOOLS");
            CheckExist("useuserlibl", "false");

            CheckExist("TREE_LIST", "");
            CheckExist("ZOOM", 12.75f.ToString());
            CheckExist("INDENT_SIZE", "4");
            CheckExist("SHOW_SPACES", "true");
            CheckExist("HIGHLIGHT_CURRENT_LINE", "true");

            CheckExist("CMPTYPES", "RPGLE|SQLRPGLE|CLLE|C|CMD");
            CheckExist("DFT_RPGLE", "CRTBNDRPG|CRTRPGMOD");
            CheckExist("DFT_SQLRPGLE", "CRTSQLRPGI|CRTSQLRPGI_MOD");
            CheckExist("DFT_CLLE", "CRTBNDCL");
            CheckExist("DFT_C", "CRTBNDC|CRTCMOD");
            CheckExist("DFT_CMD", "CRTCMD");

            CheckExist("TYPE_RPGLE", "CRTBNDRPG|CRTRPGMOD");
            CheckExist("CRTBNDRPG", "CRTBNDRPG PGM(&OPENLIB/&OPENMBR) SRCFILE(&OPENLIB/&OPENSPF) OPTION(*EVENTF) DBGVIEW(*SOURCE)");
            CheckExist("CRTRPGMOD", "CRTRPGMOD MODULE(&OPENLIB/&OPENMBR) SRCFILE(&OPENLIB/&OPENSPF) OPTION(*EVENTF)");

            CheckExist("TYPE_SQLRPGLE", "CRTSQLRPGI|CRTSQLRPGI_MOD");
            CheckExist("CRTSQLRPGI", "CRTSQLRPGI OBJ(&OPENLIB/&OPENMBR) SRCFILE(&OPENLIB/&OPENSPF) COMMIT(*NONE) OPTION(*EVENTF *XREF)");
            CheckExist("CRTSQLRPGI_MOD", "CRTSQLRPGI OBJ(&OPENLIB/&OPENMBR) SRCFILE(&OPENLIB/&OPENSPF) COMMIT(*NONE) OBJTYPE(*MODULE) OPTION(*EVENTF *XREF)");

            CheckExist("TYPE_CLLE", "CRTBNDCL");
            CheckExist("CRTBNDCL", "CRTBNDCL PGM(&OPENLIB/&OPENMBR) SRCFILE(&OPENLIB/&OPENSPF) OPTION(*EVENTF)");

            CheckExist("TYPE_C", "CRTBNDC|CRTCMOD");
            CheckExist("CRTBNDC", "CRTBNDC PGM(&OPENLIB/&OPENMBR) SRCFILE(&OPENLIB/&OPENSPF) DBGVIEW(*SOURCE) OPTION(*EVENTF)");
            CheckExist("CRTCMOD", "CRTCMOD MODULE(&OPENLIB/&OPENMBR) SRCFILE(&OPENLIB/&OPENSPF) DBGVIEW(*SOURCE) OPTION(*EVENTF)");

            CheckExist("TYPE_CMD", "CRTCMD");
            CheckExist("CRTCMD", "CRTCMD CMD(&OPENLIB/&OPENMBR) PGM(&OPENLIB/&OPENMBR) SRCFILE(&OPENLIB/&OPENSPF)");

            CheckExist("LIBSAVE", "");

            SaveConfig();
        }

        public void SaveConfig()
        {
            List<string> fileout = new List<string>();
            foreach (var key in Data.Keys)
            {
                fileout.Add(key + '=' + Data[key]);
            }
            File.WriteAllLines(ConfigLocation, fileout.ToArray());
        }

        public string GetValue(string Key)
        {
            if (Data.ContainsKey(Key))
                return Data[Key];
            else
                return "";
        }

        public void SetValue(string Key, string Value)
        {
            if (Data.ContainsKey(Key))
                Data[Key] = Value;
            else
                Data.Add(Key, Value);

            SaveConfig();
        }
    }
}
