using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Enigma
{
    public partial class EnigmaForm : Form
    {
        public int spacecount = 0;
        public string plugboardalphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public string rotorI = "EKMFLGDQVZNTOWYHXUSPAIBRCJ";
        public string rotorII = "AJDKSIRUXBLHWTMCQGZNPYFVOE";
        public string rotorIII = "BDFHJLCPRTXVZNYEIWGAKMUSQO";
        public string reflectorB = "YRUHQSLDPXNGOKMIEBFZCWVJAT";
        public string reflectorC = "FVPJIAOYEDRZXWGCTKUQSBNMHL";
        public EnigmaForm()
        {
            InitializeComponent();

            rtr_alpha_right.SelectedIndex = 0;
            rtr_alpha_middle.SelectedIndex = 0;
            rtr_alpha_left.SelectedIndex = 0;

            rtr_type_right.SelectedIndex = 0;
            rtr_type_middle.SelectedIndex = 0;
            rtr_type_left.SelectedIndex = 0;

            rtr_ring_right.SelectedIndex = 0;
            rtr_ring_middle.SelectedIndex = 0;
            rtr_ring_left.SelectedIndex = 0;

            rfl_type.SelectedIndex = 0;

        }

        private void btn_input_Click(object sender, EventArgs e) //On click of any of keyboard buttons
        {
            //Works out which letter was pressed
            Button button = sender as Button;
            var inputletter = button.Name.Substring(button.Name.Length - 1);
            tbx_input.AppendText(inputletter.ToUpper());  
            encryptMessage(inputletter);
        }

        private void encryptMessage(string inputletter)
        {
            string outputletter;
            inputletter = inputletter.ToUpper();

            //Plugboard Substitution
            outputletter = plugboard_Substitution(Convert.ToChar(inputletter)); //outputletter is uppercase

            //Rotors + Reflector + Rotors 
            rotor_Movement();
            outputletter = rotor_Encryption(Convert.ToChar(outputletter), 0, true);
            outputletter = reflector(Convert.ToChar(outputletter));
            outputletter = rotor_Encryption(Convert.ToChar(outputletter), 0, false);

            //Plugboard Substitution
            outputletter = plugboard_Substitution(Convert.ToChar(outputletter));

            //Output Message
            output_Message(Convert.ToString(outputletter));
        }

        private void output_Message(string outputletter)
        {
            //Highlights and outputs the output letter
            var outputbtnname = "btn_output_" + outputletter.ToLower();
            Button button2 = this.Controls.Find(outputbtnname, true).FirstOrDefault() as Button;
            button2.BackColor = Color.Pink;
            tbx_output.AppendText(outputletter);
            spacecount = (spacecount+1)%5;
            if(spacecount == 0)
            {
                tbx_output.AppendText(" ");
                spacecount = 0;
            }

            //Timer to unhighlight letter
            var timer = new Timer() { Interval = 240, Enabled = true, };
            timer.Tick += (s, f) =>
                button2.BackColor = Color.Transparent;
        }



        //PLUGBOARD
        private void tbx_plug_TextChanged(object sender, EventArgs e)
        {
            TextBox textbox = sender as TextBox;
            var plugletter = textbox.Name.Substring(textbox.Name.Length - 1);
            var swaplettername = "";
            var swappedletter = "";

            if (textbox.Text.Length > 1) //Error message if more than one letter typed
            {
                MessageBox.Show("Enter only one letter");
                textbox.Text = "";
            }
            else if (textbox.Text == " " || !textbox.Text.All(Char.IsLetter)) //Validation for letter only
            {
                MessageBox.Show("Enter a letter");
                textbox.Text = "";
            }
            else if (textbox.Text != "") //Swaps the corresponding letter textbox
            {
                swappedletter = textbox.Text.ToLower();

                //Check to see if any other textboxes have this letter
                bool found = false;
                int count = 0;
                while (found == false && count < 26)
                {
                    int ascii = count + 97;
                    char charascii = (char)ascii;
                    var checktextname = "tbx_plug_" + charascii;
                    var untruechecktextname = "tbx_plug_" + swappedletter;
                    TextBox checktext = this.Controls.Find(checktextname, true).FirstOrDefault() as TextBox;

                    if (checktext.Text.ToLower() == swappedletter && checktextname != untruechecktextname && checktextname != textbox.Name)
                    {
                        MessageBox.Show("This letter is already in use");
                        textbox.Text = "";
                        found = true;
                    }
                    count++;
                }

                ////Swaps the corresponding textbox if it is not in use
                if (found == false)
                {
                    swaplettername = "tbx_plug_" + swappedletter.ToLower();
                    TextBox swaptext = this.Controls.Find(swaplettername, true).FirstOrDefault() as TextBox;
                    swaptext.Text = plugletter.ToUpper();
                }

            }

        }

        private void btn_plugboard_Click(object sender, EventArgs e)
        {
            plugboardalphabet = "";
            for (int i = 0; i < 26; i++)
            {
                int ascii = i + 97;
                char charascii = (char)ascii;
                var checktextname = "tbx_plug_" + charascii;
                TextBox checktext = this.Controls.Find(checktextname, true).FirstOrDefault() as TextBox;
                if (checktext.Text == "" || checktext.Text == " " || !checktext.Text.All(Char.IsLetter))
                {
                    MessageBox.Show("Plugboard Settings Invalid");
                }
                else
                {
                    checktext.Text = checktext.Text.ToUpper();
                    plugboardalphabet = plugboardalphabet + checktext.Text;
                }
            }
        }

        private string plugboard_Substitution(char inputletter) 
        {
            int place = (int)inputletter - 65;
           
            string outputletter = Convert.ToString(inputletter);
            char charoutputletter;

            if (plugboardalphabet.IndexOf(inputletter) != place)
            {
                charoutputletter = (char)(plugboardalphabet.IndexOf(outputletter)+65);
                outputletter = Convert.ToString(charoutputletter);
            }

            return outputletter;
        }

        //ROTORS
        private void rotor_Movement()
        {
            int rtr_right_notch = rotor_Notch("rtr_type_right");
            int rtr_middle_notch = rotor_Notch("rtr_type_middle");

            int right_index = rtr_alpha_right.SelectedIndex;
            int middle_index = rtr_alpha_middle.SelectedIndex;
            int left_index = rtr_alpha_left.SelectedIndex;

            //Setting boolean values (middle one is different because of double stepping)
            bool middleturn = false;
            bool leftturn = false;
            if (middle_index == rtr_middle_notch)
            {
                leftturn = true;
                middleturn = true;
            }           

            //Right Rotor
            
            if(right_index == rtr_right_notch)
            {
                middleturn = true;
            }          
            rtr_alpha_right.SelectedIndex = (right_index + 1) % 26;

            //Middle Rotor
            if (middleturn == true)
            {                   
                rtr_alpha_middle.SelectedIndex = (middle_index + 1) % 26;
                middleturn = false;
            }

            //Left Rotor
            if (leftturn == true)
            { 
                rtr_alpha_left.SelectedIndex = (left_index + 1) % 26;
                leftturn = false;
            }
        }

        private int rotor_Notch(string rtr_type_name)
        {
            DomainUpDown rtr_name = this.Controls.Find(rtr_type_name, true).FirstOrDefault() as DomainUpDown;
            int rotor_type = rtr_name.SelectedIndex;
            if (rotor_type == 0)
                {
                return 16;
                }
            else if (rotor_type == 1)
            {
                return 4;
            }
            else if (rotor_type == 2)
            {
                return 21;
            }
            else
            {
                return 25;
            }
        }

        private string rotor_Encryption(char inputletter, int count, bool input)
        {
            int rotor_type = 0;
            string outputletter = Convert.ToString(inputletter);

            while (count < 3)
            {
                //Goes from right to left finding the rotor type of each rotor.
                if((input && count == 0) || (!input && count==2))
                {
                    rotor_type = rtr_type_right.SelectedIndex;
                }
                else if (count == 1)
                {
                    rotor_type = rtr_type_middle.SelectedIndex;
                }
                else if ((input && count == 2) || (!input && count == 0))
                {
                    rotor_type = rtr_type_left.SelectedIndex;
                }

                //Gets the substitution alphabet for each rotor type
                string sub_alphabet = "";
                if (rotor_type == 0)
                {
                    sub_alphabet = rotorI;
                }
                else if (rotor_type == 1)
                {
                    sub_alphabet = rotorII;
                }
                else if (rotor_type == 2)
                {
                    sub_alphabet = rotorIII;
                }

                //Does the substitution
                
                int place = (int)inputletter - 65;
                int display_place = 0;
                int ring_setting = 0;

                //Gets the display window + ring setting of each rotor and calculates the rotor entry point of the input
                if ((input && count == 0) || (!input && count == 2))
                {
                    display_place = rtr_alpha_right.SelectedIndex;
                    ring_setting = rtr_ring_right.SelectedIndex;
                }
                else if (count == 1)
                {
                    display_place = rtr_alpha_middle.SelectedIndex;
                    ring_setting = rtr_ring_middle.SelectedIndex;

                }
                else if ((input && count == 2) || (!input && count == 0))
                {
                    display_place = rtr_alpha_left.SelectedIndex;
                    ring_setting = rtr_ring_left.SelectedIndex;                   
                }

                int shift = floor_Mod(display_place - ring_setting, 26);
                place = (place + shift) % 26;

                //Options depending on whether the encryption is being inputted or reflected
                if(input)
                {
                    inputletter = Convert.ToChar(sub_alphabet.Substring(place, 1));
                }
                else
                {
                    place = sub_alphabet.IndexOf((char)(place + 65));
                    inputletter = (char)(place + 65);
                }
                
                place = (int)inputletter - 65;
                
                //Converts the input back into normal alphabet input before it is entered            
                int new_input = floor_Mod(place-shift, 26) + 65;
                inputletter = (char)new_input;
                outputletter = Convert.ToString(inputletter);

                //Recursive to go through all 3 rotors
                count++;         
                rotor_Encryption(inputletter, count, input);
                
            }
            return outputletter;
        }

        //REFLECTOR
        private string reflector(char inputletter)
        {
            int type = rfl_type.SelectedIndex;
            string sub_alphabet = "";
            string outputletter = Convert.ToString(inputletter);
            if(type == 0)
            {
                sub_alphabet = reflectorB;
            }
            else
            {
                sub_alphabet = reflectorC;
            }
            int place = (int)inputletter - 65;
            outputletter = sub_alphabet.Substring(place, 1);
            return outputletter;
        }

        //Floored Division Modulo
        public int floor_Mod(int dividend, int divisor)
        {
            int floor_division = Convert.ToInt32(Math.Floor((double)dividend / divisor));
            int floor_modulus = dividend-divisor*floor_division;
            return floor_modulus;
        }

        //RESET SETTINGS
        private void btn_reset_Click(object sender, EventArgs e)
        {
            spacecount = 0;
            plugboardalphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            //Reset Textboxes
            tbx_input.Text = "";
            tbx_output.Text = "";

            //Reset Rotors
            rtr_alpha_right.SelectedIndex = 0;
            rtr_alpha_middle.SelectedIndex = 0;
            rtr_alpha_left.SelectedIndex = 0;

            rtr_type_right.SelectedIndex = 0;
            rtr_type_middle.SelectedIndex = 0;
            rtr_type_left.SelectedIndex = 0;

            rtr_ring_right.SelectedIndex = 0;
            rtr_ring_middle.SelectedIndex = 0;
            rtr_ring_left.SelectedIndex = 0;

            //Reset Reflector
            rfl_type.SelectedIndex = 0;

            //Reset Plugboard
            for (int i = 0; i < 26; i++)
            {
                int ascii = i + 97;
                char charascii = (char)ascii;
                string letter = Convert.ToString(charascii);
                var checktextname = "tbx_plug_" + letter;
                TextBox checktext = this.Controls.Find(checktextname, true).FirstOrDefault() as TextBox;
                checktext.Text = "";
            }
            for (int i = 0; i < 26; i++)
            {
                int ascii = i + 97;
                char charascii = (char)ascii;
                string letter = Convert.ToString(charascii);
                var checktextname = "tbx_plug_" + letter;
                TextBox checktext = this.Controls.Find(checktextname, true).FirstOrDefault() as TextBox;
                checktext.Text = letter.ToUpper();
            }
        }
    }
}