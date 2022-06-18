
namespace _1911_Anti_Cheat
{
    partial class AntiCheatForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrAntiCheat = new System.Windows.Forms.Timer(this.components);
            this.txtCaptain = new System.Windows.Forms.TextBox();
            this.txtPlayer = new System.Windows.Forms.TextBox();
            this.btnReady = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tmrFindGame = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrAntiCheat
            // 
            this.tmrAntiCheat.Interval = 1000;
            this.tmrAntiCheat.Tick += new System.EventHandler(this.tmrAntiCheat_Tick);
            // 
            // txtCaptain
            // 
            this.txtCaptain.Location = new System.Drawing.Point(119, 20);
            this.txtCaptain.Name = "txtCaptain";
            this.txtCaptain.Size = new System.Drawing.Size(196, 23);
            this.txtCaptain.TabIndex = 0;
            // 
            // txtPlayer
            // 
            this.txtPlayer.Location = new System.Drawing.Point(119, 49);
            this.txtPlayer.Name = "txtPlayer";
            this.txtPlayer.Size = new System.Drawing.Size(196, 23);
            this.txtPlayer.TabIndex = 2;
            // 
            // btnReady
            // 
            this.btnReady.Enabled = false;
            this.btnReady.Location = new System.Drawing.Point(174, 89);
            this.btnReady.Name = "btnReady";
            this.btnReady.Size = new System.Drawing.Size(141, 31);
            this.btnReady.TabIndex = 4;
            this.btnReady.Text = "Waiting for Game...";
            this.btnReady.UseVisualStyleBackColor = true;
            this.btnReady.Click += new System.EventHandler(this.btnReady_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Captain";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Player Name";
            // 
            // tmrFindGame
            // 
            this.tmrFindGame.Enabled = true;
            this.tmrFindGame.Interval = 1000;
            this.tmrFindGame.Tick += new System.EventHandler(this.tmrFindGame_Tick);
            // 
            // AntiCheatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 131);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReady);
            this.Controls.Add(this.txtPlayer);
            this.Controls.Add(this.txtCaptain);
            this.Name = "AntiCheatForm";
            this.Text = "1911 Anti Cheat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnExit);
            this.Load += new System.EventHandler(this.OnStartup);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmrAntiCheat;
        private System.Windows.Forms.TextBox txtCaptain;
        private System.Windows.Forms.TextBox txtPlayer;
        private System.Windows.Forms.Button btnReady;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer tmrFindGame;
    }
}

