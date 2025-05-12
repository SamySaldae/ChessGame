using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[,] Mat = new string[8, 8];
        enum enSelection { PosFrom, PosTo, NothingSelected };
       
        enum enComponent { enTour, enHorse, enBishop, enQueen, enKing, enPawn, enUnknown };
        private bool is_valide_move_for_horse()
        {
            int dx = Math.Abs((Order.act_X - Order.To_X));
            int dy = Math.Abs((Order.act_Y - Order.To_Y));
            return (dx == 1 && dy == 2 || dx == 2 && dy == 1);
        }
        private bool is_valide_move_for_Tower()
        {
            return ((Order.act_X == Order.To_X || Order.act_Y == Order.To_Y) &&
                Make_sur_that_way_is_free(Get_Nature_OfMove()));
        }
        private bool is_valide_move_for_bishop()
        {
            return ((Math.Abs(Order.act_Y - Order.To_Y) == Math.Abs(Order.act_X - Order.To_X))
                 && Make_sur_that_way_is_free(Get_Nature_OfMove()));

        }
        private bool is_valide_for_queen()
        {
            return (is_valide_move_for_Tower() || is_valide_move_for_bishop());
        }
        private bool is_valide_for_king()
        {
            int dx = Math.Abs((Order.act_X - Order.To_X));
            int dy = Math.Abs((Order.act_Y - Order.To_Y));
            return (dx <= 1 && dy <= 1);
        }
        enum enPlayer {enPlayer_Blanc,enPlayer_Noir};
        enPlayer enActPlayer;
        private bool Component_of_PlayerBlanc_Selected()
        {
            if (is_empty(Order.act_X, Order.act_Y)) return false;
            else if (Mat[Order.act_X, Order.act_Y][2] == '2') return false;
            return true;
        }
        private bool Component_of_PlayerNoir_Selected()
        {
            return (!Component_of_PlayerBlanc_Selected() && !is_empty(Order.act_X, Order.act_Y));
        }
        
        enum enPawnColor { enWhite,enBlack };
        enum enMoveDirection { UP, DOWN , LEFT , RIGHT , UP_RIGHT ,UP_LEFT , DOWN_RIGHT,DOWN_LEFT };
        private enMoveDirection Get_Nature_OfMove()
        { 
            //Move for bishop and tour and queen 
            int X1 = Order.act_X; int X2= Order.To_X;
            int Y1 = Order.act_Y; int Y2= Order.To_Y;
            if (X1 - X2 == 0 && Y1 - Y2 > 0)
                return enMoveDirection.LEFT;
            else if (X1 - X2 == 0 && Y1 - Y2 < 0)
                return enMoveDirection.RIGHT;
            else if (X1 - X2 > 0 && Y1 - Y2 == 0)
                return enMoveDirection.UP;
            else if (X1 - X2 < 0 && Y1 - Y2 == 0)
                return enMoveDirection.DOWN;
            else if (X1 - X2 > 0 && Y1 - Y2 > 0)
                return enMoveDirection.UP_LEFT;
            else if (X1 - X2 < 0 && Y1 - Y2 > 0)
                return enMoveDirection.DOWN_LEFT;
            else if (X1 - X2 > 0 && Y1 - Y2 < 0)
                return enMoveDirection.UP_RIGHT;
            else if (X1 - X2 < 0 && Y1 - Y2 < 0)
                return enMoveDirection.DOWN_RIGHT;
            else
                return enMoveDirection.RIGHT; //Probably not will be used
        }
        private void Transform_Pawn_Into_Queen()
        {
            for(int j = 0;j<8;j++)
            {
                if (Mat[0, j] == "PB1")
                    Mat[0, j] = "RB1";
                else if(Mat[0, j] == "PN1")
                    Mat[0, j] = "RN1";

                if (Mat[7, j] == "PB2")
                    Mat[7, j] = "RB2";
                else if(Mat[7, j] == "PN2")
                    Mat[7, j] = "RN2";
            }
        }
        private bool Same_Position(int X1, int X2,int Y1, int Y2)
        {
            return (X1 == X2 && Y1 == Y2);
        }
        private bool Make_sur_that_way_is_free(enMoveDirection Move_Nature)
        {
            int Local_X_Checker = Order.To_X;
            int Local_Y_Checker = Order.To_Y;
            do
            {
                switch (Move_Nature)
                {
                    case enMoveDirection.LEFT: Local_Y_Checker++; break;
                    case enMoveDirection.RIGHT: Local_Y_Checker--; break;
                    case enMoveDirection.UP: Local_X_Checker++; break; 
                    case enMoveDirection.DOWN: Local_X_Checker--; break;
                    case enMoveDirection.DOWN_LEFT: Local_Y_Checker++; Local_X_Checker--; break;
                    case enMoveDirection.DOWN_RIGHT: Local_Y_Checker--; Local_X_Checker--; break;
                    case enMoveDirection.UP_LEFT: Local_Y_Checker++; Local_X_Checker++; break;
                    case enMoveDirection.UP_RIGHT: Local_Y_Checker--; Local_X_Checker++; break;
                }
                if (Same_Position(Order.act_X, Local_X_Checker, Order.act_Y, Local_Y_Checker))
                {
                    return true;
                }
                if (!is_empty(Local_X_Checker, Local_Y_Checker))
                    return false;

            } while (!Same_Position(Order.act_X, Local_X_Checker, Order.act_Y, Local_Y_Checker));
            return true;
        }
        private bool is_black_component(int X,int Y)
        {
            return (Mat[Order.act_X, Order.act_Y][2] == '2');
        }
        private bool is_white_component(int X, int Y)
        {
            return (Mat[Order.act_X, Order.act_Y][2] == '1');
        }
        private bool is_enemy(int X,int Y,enPawnColor Color)
        {
            if(Color == enPawnColor.enWhite)
                return is_black_component(X, Y);
            else if(!is_empty(X,Y) && Color == enPawnColor.enBlack)
                return is_white_component(X,Y);
            return false;
        }
        private bool enemy_is_onSideOF_PAWN(int X,int Y,enPawnColor Color)
        {

            int limits = 0;
            int next_step = 1;
            if(Color == enPawnColor.enWhite)
            {
                limits = 0;
                next_step = 1;
            }
            else
            {
                limits = 7;
                next_step = - 1;
            }

            if (Y > 1 && Y<7 && X!= limits) //Macro centered
            {
                return (is_enemy(X - next_step, Y + 1,Color) || is_enemy(X - next_step, Y - 1,Color)); //check two sides
            }
            else if (Y == 0 && X != limits)
            {
                return is_enemy(X - next_step, Y + 1,Color); //check only right side
            }
            else if (Y == 7 && X != limits)
            {
                return is_enemy(X - next_step, Y - 1, Color); //check only left side 
            }
            else if (X == limits)
            {
                return false; //pawn is on his limits 
            }
            else return false;
        }
       private bool is_valide_for_pawn(enPlayer player)
        {
            int X1 = Order.act_X; int Y1=Order.act_Y;
            int X2 = Order.To_X; int Y2= Order.To_Y;
            if (player == enPlayer.enPlayer_Blanc) //Begining of moving pawn
            {
                if (((X1 == 6 && X2 == 4) || (X1 == X2 + 1)) && (!enemy_is_onSideOF_PAWN(X1, Y1,enPawnColor.enWhite)
                    && is_empty(X2, Y2)) && (Y1 == Y2))  //Clean first move
                    return true;
                else if ((X1 ==X2 + 1) && (enemy_is_onSideOF_PAWN(X1,Y1, enPawnColor.enWhite) && (Math.Abs(Y1-Y2) == 1)))
                    return true; //Attack ennemy on my door on right & left side
           
                else if (X2 == X1 - 1 && X1!=6 && is_empty(X2, Y2) && Y1 == Y2) // simple move
                    return true;
                else
                    return false;
            }
            if (player == enPlayer.enPlayer_Noir) //Begining of moving pawn
            {
                if ((X1 == 1 && X2 == 3) || (X1==X2-1) && (!enemy_is_onSideOF_PAWN(X1, Y1, enPawnColor.enBlack)
                    && is_empty(X2, Y2)) && (Y1 == Y2))  //Clean first move
                    return true;
                else if ((X1 == X2 - 1) && (enemy_is_onSideOF_PAWN(X1, Y1, enPawnColor.enBlack) && (Math.Abs(Y1 - Y2) == 1)))
                    return true; //Attack ennemy on my door on right & left side

                else if (X2 == X1 + 1 && X1 != 1 && is_empty(X2, Y2) && Y1 == Y2) // simple move
                    return true;
                else
                    return false;
            }
            {
                return false;
            }
        }
        enPlayer Act_Player;
        private void change_player()
        {
            if (Act_Player == enPlayer.enPlayer_Blanc)
                Act_Player = enPlayer.enPlayer_Noir;
            else
                Act_Player = enPlayer.enPlayer_Blanc;
        }
        private enComponent From_MatIndex_To_Component(string Indx) //its runs from Act_x && Act_y
        {
            char Caracter = Indx[0];
            switch (Caracter)
            {
                case 'T':
                    return enComponent.enTour;
                case 'C':
                    return enComponent.enHorse;
                case 'F':
                    return enComponent.enBishop;
                case 'R':
                    return enComponent.enQueen;
                case 'G':
                    return enComponent.enKing;
                case 'P':
                    return enComponent.enPawn;
            }
            return enComponent.enUnknown; //we will not arrive here 
        }

        private bool check_valide_move()
        {
            switch (Component_Selected)
            {
                case enComponent.enTour:
                    return is_valide_move_for_Tower();
                case enComponent.enHorse:
                    return is_valide_move_for_horse();
                case enComponent.enBishop:
                    return is_valide_move_for_bishop();
                case enComponent.enQueen:
                    return is_valide_for_queen();
                case enComponent.enKing:
                    return is_valide_for_king();
                case enComponent.enPawn:
                    return is_valide_for_pawn(Act_Player);
                default: return false;
            }
        }
        private struct stPlayer
        {
            public
            enPlayer Player;
            public
            int horses;
            public
            int pawns;
            public
            int tours;
            public
            int bishops;
            public
            int Queens;
            public
            bool King_alive;
        }
        stPlayer Player_Blanc, Player_Noir;
       //Initilise parameters for the two players
        enComponent Component_Selected;
        public struct stOrder
        {
            public
             int act_X;
            public
             int act_Y;
            public
                 int To_X;
            public
                int To_Y;
            public
                string str_component_selected;

        };
        bool invert_bool(bool is_white)
        {
            if (is_white)
                return false;
            return true;
        }
        void PrepareWhiteAndBlackCases()
        {
            bool is_white = true;

            for (int i = 2; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (is_white)
                        Mat[i, j] = "cB";
                    else Mat[i, j] = "cN";

                    is_white = invert_bool(is_white);
                    if (j == 7)
                        is_white = invert_bool(is_white);
                }
            }
        }
        private void FromMatroxToPictureBox()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)

                {
                    PictureBox pb = this.Controls.Find("pb" + i + j, true).FirstOrDefault() as PictureBox;
                    if (pb != null)
                    {
                        pb.Image = Image.FromFile(@"C:\\Users\\khodi\\Videos\\PICTURES - CHESS\\" + Mat[i, j] + ".jpg");
                        pb.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] arr0 = { "TB2", "CN2", "FB2", "RN2", "GB2", "FN2", "CB2", "TN2" };
            string[] arr1 = { "PN2", "PB2", "PN2", "PB2", "PN2", "PB2", "PN2", "PB2" };
            string[] arr6 = { "PB1", "PN1", "PB1", "PN1", "PB1", "PN1", "PB1", "PN1" };
            string[] arr7 = { "TN1", "CB1", "FN1", "RB1", "GN1", "FB1", "CN1", "TB1" };

            for (int i = 0; i < 8; i++)
            {
                Mat[0, i] = arr0[i];
            }
            for (int i = 0; i < 8; i++)
            {
                Mat[1, i] = arr1[i];
            }
            for (int i = 0; i < 8; i++)
            {
                Mat[6, i] = arr6[i];
            }
            for (int i = 0; i < 8; i++)
            {
                Mat[7, i] = arr7[i];
            }
            PrepareWhiteAndBlackCases();
            FromMatroxToPictureBox();
            Act_Player = enPlayer.enPlayer_Blanc;
        }
        enSelection Selection = enSelection.NothingSelected;
        stOrder Order;
        private bool is_empty(int X, int Y)
        {
            return (Mat[X, Y] == "cN" || Mat[X, Y] == "cB");
        }
        private bool MakeOrder(PictureBox pb)
        {
            if (pb.Name == "cN" || pb.Name == "cB")
                return false;
            else
            {
                
                string index = pb.Name;
                int i = index[2] - '0';
                int j = index[3] - '0';
                Order.str_component_selected = Mat[i, j];

                Order.act_X = i;
                Order.act_Y = j;

                ActX.Text = Order.act_X.ToString();
                ActY.Text = Order.act_Y.ToString();

                Component_Selected = From_MatIndex_To_Component(Mat[Order.act_X, Order.act_Y]);
                Encolor_Selected_Component();
                if (Component_Selected == enComponent.enUnknown)
                    return false;
                return true;
            }
        }
        private char Generate_NameOfCase()
        {
            switch (Component_Selected)
            {
                case enComponent.enPawn:
                    return 'P';
                case enComponent.enQueen:
                    return 'R';
                case enComponent.enKing:
                    return 'G';
                case enComponent.enBishop:
                    return 'F';
                case enComponent.enHorse:
                    return 'C';
                case enComponent.enTour:
                    return 'T';
            }
            return 'O';
        }
        private bool Player_Didnot_moved()
        {
            return Same_Position(Order.act_X, Order.To_X,Order.act_Y,Order.To_Y);
        }
        private bool check_valide_compoment()
        {
            if (Mat[Order.act_X, Order.act_Y].ToString() == "cN" || Mat[Order.act_X, Order.act_Y].ToString() == "cB")
                return false;
            return true;
        }
        private void Replace_Last_Position_By_Empty_Case_By_Color(char color)
        {
            if (color == 'N')
                Mat[Order.act_X, Order.act_Y] = "cN";
            else
                Mat[Order.act_X, Order.act_Y] = "cB";
        }
        private void execute_move()
        {

            char color_destination = Detect_Color_of_case(Order.To_X, Order.To_Y); //Color of destination case 
            char color_from = Detect_Color_of_case(Order.act_X, Order.act_Y);

            Mat[Order.To_X, Order.To_Y] = Mat[Order.act_X, Order.act_Y];
            //Text Box Color
            Set_Color_Case_With_Color(Order.To_X,Order.To_Y,color_destination); //Set Destination
            Replace_Last_Position_By_Empty_Case_By_Color(color_from);
            Transform_Pawn_Into_Queen();
        }
    
        private void Refresh_UI()
        {
            FromMatroxToPictureBox();
        }
        private void Encolor_Selected_Component()
        {
            string NewPictureName;
            if (!is_empty(Order.act_X, Order.act_Y) && Selection == enSelection.PosFrom)
            {
                NewPictureName = 'S' + Mat[Order.act_X, Order.act_Y];
                PictureBox pb = this.Controls.Find("pb" + Order.act_X + Order.act_Y, true).FirstOrDefault() as PictureBox;
                if (pb != null)
                {
                    pb.Image = Image.FromFile(@"C:\\Users\\khodi\\Videos\\PICTURES - CHESS\\" + 'S' + Mat[Order.act_X, Order.act_Y] + ".jpg");
                    pb.SizeMode = PictureBoxSizeMode.CenterImage;
                }
            }
            
        }
        private char Detect_Color_of_case(int X, int Y)
        {
            return Mat[X, Y][1];
        }
        private string assing_CharToString(string Str, char new_caracter, int Pos)
        {
            char[] chars = Str.ToCharArray();
            chars[Pos] = new_caracter;
            Str = new string(chars);
            return Str;
        }
        private void Set_Color_Case_With_Color(int X,int Y,char color)
        {
            
            Mat[X, Y] = assing_CharToString(Mat[X, Y], color, 1);
        }
        private bool Member_Is_moving_into_ally()
        {
            int X1 = Order.act_X; int Y1=Order.act_Y;
            int X2 = Order.To_X; int Y2=Order.To_Y;
           return (Mat[X1, Y1][2] == Mat[X2, Y2][2]);
        }
        private void MakeMove(PictureBox pb)
        {
            string index = pb.Name;
            int i = index[2] - '0';
            int j = index[3] - '0';
            Order.To_X = i;
            Order.To_Y = j;
            
            
            ToX.Text = Order.To_X.ToString();
            ToY.Text = Order.To_Y.ToString();

            if ((check_valide_move()) && (is_empty(Order.To_X, Order.To_Y) || !Member_Is_moving_into_ally())
            && !Player_Didnot_moved())
            {
                execute_move();
                change_player();
                retry = false;
                Refresh_UI();
            }
            else
            {
                retry = true;
                Refresh_UI();
            }
        }
        private bool Player_Respected_his_turn()
        {
            if (Component_of_PlayerBlanc_Selected() && Act_Player == enPlayer.enPlayer_Blanc)
                return true;
            else if(Component_of_PlayerNoir_Selected() && Act_Player == enPlayer.enPlayer_Noir)
                return true;
            return false;
        }
        private void ClickOnBox(object sender, MouseEventArgs e)
        {
            if (Selection == enSelection.NothingSelected)
            {
                Selection = enSelection.PosFrom;
                if (!MakeOrder((PictureBox)sender) || !Player_Respected_his_turn())
                    Selection = enSelection.NothingSelected;
            }
            else if (Selection == enSelection.PosFrom)
            {
                Selection = enSelection.PosTo;
                MakeMove((PictureBox)sender);
                Selection = enSelection.NothingSelected;
            }
        }
        bool retry = false;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ActY_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
