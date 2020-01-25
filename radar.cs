using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace OnlyRadar
{
    class Program
    {
        public static bool isConf = true;
        public const Int32 m_fFlags = 0x104;
        public const Int32 dwLocalPlayer = 0xD29B1C;
        public const Int32 m_iTeamNum = 0xF4;
        public const Int32 dwEntityList = 0x4D3D5DC;
        public const Int32 m_bDormant = 0xED;

        public const Int32 fJump = 0x51E0E00;

        public const Int32 m_bSpotted = 0x93D;

        public static string process = "csgo";
        public static int bClient;
        public static int bEngine;

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetAsyncKeyState(int vKey);
        static void Main(string[] args)
        {
            Console.Title = Guid.NewGuid().ToString("N");
            
            if(isConf)
            {
                VAMemory vam = new VAMemory(process);

                while (!getMod())
                {
                    Console.Clear();
                    Console.Write("Waiting for the process ", Console.ForegroundColor = ConsoleColor.Yellow);
                    Thread.Sleep(500);
                    Console.Write(". ", Console.ForegroundColor = ConsoleColor.Yellow);
                    Thread.Sleep(500);
                    Console.Write(". ", Console.ForegroundColor = ConsoleColor.Yellow);
                    Thread.Sleep(500);
                    Console.WriteLine(". ", Console.ForegroundColor = ConsoleColor.Yellow);
                    Thread.Sleep(500);
                }
                Console.WriteLine("CS:GO process found", Console.ForegroundColor = ConsoleColor.Yellow);
                Console.WriteLine("Waiting 2 seconds...", Console.ForegroundColor = ConsoleColor.Yellow);
                Thread.Sleep(2000);
                Console.Clear();
                Console.WriteLine("RadarHack turned on", Console.ForegroundColor = ConsoleColor.White, Console.BackgroundColor = ConsoleColor.Red);
                int address = bClient + dwLocalPlayer;
                int LocalPlayer = vam.ReadInt32((IntPtr)address);
                while(true)
                {
                    try
                    {
                        address = bClient + dwLocalPlayer;
                        LocalPlayer = vam.ReadInt32((IntPtr)address);
                        int PlayerInCross;
                        int MyTeam;
                        int aFlags = LocalPlayer + m_fFlags;

                        while (GetAsyncKeyState(32) < 0)
                        {
                            int Flags = vam.ReadInt32((IntPtr)aFlags);
                            if (Flags == 257)
                            {
                                vam.WriteInt32((IntPtr)fJump, 5);
                                vam.WriteInt32((IntPtr)fJump, 4);
                            }
                        }

                        int i = 1;
                            do
                            {
                                address = bClient + dwLocalPlayer;
                                LocalPlayer = vam.ReadInt32((IntPtr)address);

                                address = LocalPlayer + m_iTeamNum;
                                MyTeam = vam.ReadInt32((IntPtr)address);

                                address = bClient + dwEntityList + (i - 1) * 0x10;
                                int PTRtoPIC = vam.ReadInt32((IntPtr)address);

                                address = PTRtoPIC + m_iTeamNum;
                                int PICTeam = vam.ReadInt32((IntPtr)address);

                                address = PTRtoPIC + m_bDormant;
                                if (!vam.ReadBoolean((IntPtr)address))
                                {
                                    if (PICTeam != MyTeam)
                                    {
                                    vam.WriteBoolean((IntPtr)PTRtoPIC + m_bSpotted, true);
                                    }
                                }
                                i++;
                            }
                            while (i < 32);
                        



                    }
                    catch(Exception ex)
                    {
                        Console.Clear();
                        Console.WriteLine(ex.Message);
                        Thread.Sleep(5000);
                    }
                    Thread.Sleep(5000);
                }
            }



        }
        
        static bool getMod()
        {
            try
            {
                Process[] p = Process.GetProcessesByName(process);
                if (p.Length > 0)
                {
                    foreach (ProcessModule m in p[0].Modules)
                    {
                        if (m.ModuleName == "client_panorama.dll")
                        {
                            bClient = (int)m.BaseAddress;
                            return true;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
