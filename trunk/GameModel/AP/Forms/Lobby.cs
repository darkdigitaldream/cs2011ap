﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetLib;

namespace AP.Forms
{
    public partial class Lobby : Form
    {
        private LobbyManager lobbyManager;
        private ClientManager clientManager;
        /// <summary>
        /// This lobby constructor is used when joining the lobby
        /// </summary>
        public Lobby()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This lobby constructor is used for creating a lobby
        /// </summary>
        /// <param name="_Name"></param>
        public Lobby(string _Name)
        {
            var port = 9999;
            InitializeComponent();
            lbl_Name.Text = _Name;
            //lobbyManager=new LobbyManager(port);
            
        }


        private void btn_Start_Click(object sender, EventArgs e)
        {
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            //lobbyManager.listener.Close();
            this.Close();
        }
    }
}