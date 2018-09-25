using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EFT;
using System.IO;
using System.Runtime.InteropServices;

// Released by TheWWorld on UnknowCheats
// I think the godmode cause my ban or maybe i was reported for for weird killings
namespace Absolutly
{
    public class Abso : MonoBehaviour
    {
        public Abso() { }

        private GameObject GameObjectHolder;

        private IEnumerable<Player> _ply;
        private IEnumerable<ExfiltrationPoint> _extractPoints;


        private float _plyNextUpdateTime;
        private float _evaNextUpdateTime;
        protected float _infoUpdateInterval = 5f; // You can reduce this value for fastest update esp but can cause lag


        private bool _isInfoMenuActive;
        private bool _showPlayersInfo;
        private bool _showExtractInfo;
        private bool _godMode;


        private float _maxVueDistance = 1200f; // View Distance (you can change it on overlay by pressing F11 and modify the red value)


        public void Load()
        {
            GameObjectHolder = new GameObject();
            GameObjectHolder.AddComponent<Abso>();

            DontDestroyOnLoad(GameObjectHolder);
        }

        public void Unload()
        {
            Destroy(GameObjectHolder);
            Destroy(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                Unload();
            }
            if (Input.GetKeyDown(KeyCode.F11))
            {
                _isInfoMenuActive = !_isInfoMenuActive;
            }
            if (_godMode)
            {
                GodMode();
            }
        }

        private void OnGUI()
        {
            if (_isInfoMenuActive)
            {
                DrawInfoMenu();
            }

            if (_showPlayersInfo && Time.time >= _plyNextUpdateTime)
            {
                _ply = FindObjectsOfType<Player>();
                _plyNextUpdateTime = Time.time + _infoUpdateInterval;
            }

            if (_showPlayersInfo)
            {
                DrawPlayers();
            }


            if (_showExtractInfo && Time.time >= _evaNextUpdateTime)
            {
                _extractPoints = FindObjectsOfType<ExfiltrationPoint>();
                _evaNextUpdateTime = Time.time + _infoUpdateInterval;
            }

            if (_showExtractInfo)
            {
                DrawExtractInfo();
            }
        }


        private void GodMode()
        {
            foreach (var player in _ply)
            {
                if (player != null)
                {
                    // Don't forget to change or GodMode not work
                    if (player.Profile.AccountId == "YOUR_ACCOUNT_ID")
                    {
                        player.Physical.Sprinting.RestoreRate = 7.0f;
                        player.Physical.Sprinting.DrainRate = 3.5f;
                        player.Skills.StrengthBuffSprintSpeedInc.Value = 2.8f;
                        player.Skills.StrengthBuffMeleeCrits.Value = true;
                        player.Skills.StrengthBuffMeleePowerInc.Value = 4f;
                        player.Skills.StrengthBuffThrowDistanceInc.Value = 5f;
                        player.Skills.MagDrillsLoadSpeed.Value = 2f;
                        player.Skills.MagDrillsUnloadSpeed.Value = 2f;
                        player.Weapon.Template.isFastReload = true;
                        player.ProceduralWeaponAnimation.Shootingg.Intensity = 0;
                        player.ProceduralWeaponAnimation.Shootingg.RecoilStrengthXY = new Vector2(0, 0);
                        player.ProceduralWeaponAnimation.Shootingg.RecoilStrengthZ = new Vector2(0, 0);
                    }
                }
            }
        }


        private void DrawExtractInfo()
        {
            try
            {
                foreach (var point in _extractPoints)
                {
                    float distanceToObject = Vector3.Distance(Camera.main.transform.position, point.transform.position);
                    var exfilContainerBoundingVector = new Vector3(
                        Camera.main.WorldToScreenPoint(point.transform.position).x,
                        Camera.main.WorldToScreenPoint(point.transform.position).y,
                        Camera.main.WorldToScreenPoint(point.transform.position).z);

                    if (exfilContainerBoundingVector.z > 0.01)
                    {
                        GUI.color = Color.green;
                        int distance = (int)distanceToObject;
                        String exfilName = point.name;
                        string boxText = $"{exfilName} - {distance}m";

                        GUI.Label(new Rect(exfilContainerBoundingVector.x - 50f, (float)Screen.height - exfilContainerBoundingVector.y, 100f, 50f), boxText);
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("evasion.txt", ex.ToString());
            }
        }


        private void DrawPlayers()
        {
            try
            {
                foreach (var player in _ply)
                {

                    float distanceToObject = Vector3.Distance(Camera.main.transform.position, player.Transform.position);
                    var playerBoundingVector = new Vector3(
                        Camera.main.WorldToScreenPoint(player.Transform.position).x,
                        Camera.main.WorldToScreenPoint(player.Transform.position).y,
                        Camera.main.WorldToScreenPoint(player.Transform.position).z);

                    if (distanceToObject <= _maxVueDistance && playerBoundingVector.z > 0.01)
                    {
                        var playerHeadVector = new Vector3(
                            Camera.main.WorldToScreenPoint(player.PlayerBones.Head.position).x,
                            Camera.main.WorldToScreenPoint(player.PlayerBones.Head.position).y,
                            Camera.main.WorldToScreenPoint(player.PlayerBones.Head.position).z);

                        float boxXOffset = Camera.main.WorldToScreenPoint(player.Transform.position).x;
                        float boxYOffset = Camera.main.WorldToScreenPoint(player.PlayerBones.Head.position).y + 10f;
                        float boxHeight = Math.Abs(Camera.main.WorldToScreenPoint(player.PlayerBones.Head.position).y - Camera.main.WorldToScreenPoint(player.Transform.position).y) + 10f;
                        float boxWidth = boxHeight * 0.65f;

                        var playerColor = GetPlayerColor(player.Side);
                        var isAi = player.Profile.Info.RegistrationDate <= 0;
                        var deadcolor = player.Profile.Health.IsAlive ? playerColor : Color.gray;



                        GUI.color = deadcolor;
                        Vlcrpc.DrawBox(boxXOffset - boxWidth / 2f, (float)Screen.height - boxYOffset, boxWidth, boxHeight, deadcolor);
                        Vlcrpc.DrawLine(new Vector2(playerHeadVector.x - 2f, (float)Screen.height - playerHeadVector.y), new Vector2(playerHeadVector.x + 2f, (float)Screen.height - playerHeadVector.y), deadcolor);
                        Vlcrpc.DrawLine(new Vector2(playerHeadVector.x, (float)Screen.height - playerHeadVector.y - 2f), new Vector2(playerHeadVector.x, (float)Screen.height - playerHeadVector.y + 2f), deadcolor);


                        var playerName = isAi ? "AI" : player.Profile.Info.Nickname;
                        float playerHealth = player.HealthController.SummaryHealth.CurrentValue / 435f * 100f;
                        string playerDisplayName = player.Profile.Health.IsAlive ? playerName : playerName + " (Dead)";
                        string playerText = $"[{(int)playerHealth}%] {playerDisplayName} [{(int)distanceToObject}m]";

                        var playerTextVector = GUI.skin.GetStyle(playerText).CalcSize(new GUIContent(playerText));
                        GUI.Label(new Rect(playerBoundingVector.x - playerTextVector.x / 2f, (float)Screen.height - boxYOffset - 20f, 300f, 50f), playerText);
                        /*
                         You can enable it for show player weapon, but cause ESP Disapear bug and i don't know why, maybe take too many time
                          
                        if (player.Profile.Health.IsAlive)
                       {
                           CurrentWeaponIDtoName($"{player.Weapon}");
                           string playerWeaponText = CurrentWeaponIDtoName($"{player.Weapon}");
                           var playerWeaponTextVector = GUI.skin.GetStyle(playerWeaponText).CalcSize(new GUIContent(playerWeaponText));
                           GUI.Label(new Rect(playerBoundingVector.x - playerWeaponTextVector.x / 2f, (float)Screen.height - boxYOffset - 28f, 320f, 50f), playerWeaponText);
                       }
                    */
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("plyzlog.txt", ex.ToString());
            }
        }

        private string CurrentWeaponIDtoName(string id)
        {
            if (id.Contains("p226"))
                return "P226R";
            else if (id.Contains("mp443"))
                return "MP443";
            else if (id.Contains("aks74"))
                return "AKS-74";
            else if (id.Contains("_mr133_"))
                return "MR-133";
            else if (id.Contains("_mr153_"))
                return "MR-153";
            else if (id.Contains("saiga12"))
                return "Saig.12";
            else if (id.Contains("akm"))
                return "AKM";
            else if (id.Contains("ak74n"))
                return "AK-7N";
            else if (id.Contains("ak74m"))
                return "AK-7M";
            else if (id.Contains("mr43e"))
                return "MR-43E";
            else if (id.Contains("model_870"))
                return "M870";
            else if (id.Contains("mosin"))
                return "Mosin";
            else if (id.Contains("_zarya_"))
                return "Zarya";
            else if (id.Contains("_pb_"))
                return "PB9x18PM";
            else if (id.Contains("_izhmeh_pm_"))
                return "Makarov";
            else if (id.Contains("saiga_9"))
                return "SaigaPM";
            else if (id.Contains("_tt_"))
                return "TT";
            else if (id.Contains("pp-9"))
                return "pp91";
            else if (id.Contains("vepr"))
                return "VEPR";
            else if (id.Contains("glock"))
                return "Glock";
            else if (id.Contains("sks"))
                return "SKS";
            else if (id.Contains("akms"))
                return "AKMS";
            else if (id.Contains("akm_vpo"))
                return "AKM VPO";
            else if (id.Contains("vepr_km"))
                return "VEPR KM VPO";
            else if (id.Contains("mp5"))
                return "MP5";
            else if (id.Contains("mp7a1"))
                return "MP7";
            else if (id.Contains("_mpx_"))
                return "MPX";
            else if (id.Contains("molot_aps"))
                return "Molot";
            else if (id.Contains("m1a"))
                return "m1a";
            else if (id.Contains("sa58"))
                return "SA58";
            else if (id.Contains("ak101"))
                return "AK101";
            else if (id.Contains("ak102"))
                return "AK102";
            else if (id.Contains("ak103"))
                return "AK103";
            else if (id.Contains("ak104"))
                return "AK104";
            else if (id.Contains("ak105"))
                return "AK105";
            else if (id.Contains("m4a1"))
                return "M4A1";
            else if (id.Contains("sv-98"))
                return "SV-98";
            else if (id.Contains("_val_"))
                return "ASVAL";
            else if (id.Contains("_vss_"))
                return "VSS";
            else if (id.Contains("rsass"))
                return "RSASS";
            else if (id.Contains("dvl-10"))
                return "DVL10";
            else
                return id;

        }

        private Color GetPlayerColor(EPlayerSide side)
        {
            switch (side)
            {
                case EPlayerSide.Bear:
                    return Color.red;
                case EPlayerSide.Usec:
                    return Color.blue;
                case EPlayerSide.Savage:
                    return Color.white;
                default:
                    return Color.white;
            }
        }

        private void DrawInfoMenu()
        {
            GUI.color = Color.black;
            GUI.Box(new Rect(100f, 100f, 190f, 190f), "");

            GUI.color = Color.white;
            GUI.Label(new Rect(180f, 110f, 150f, 20f), "Options");

            _showPlayersInfo = GUI.Toggle(new Rect(110f, 140f, 120f, 20f), _showPlayersInfo, "Joueurs");
            _showExtractInfo = GUI.Toggle(new Rect(110f, 160f, 120f, 20f), _showExtractInfo, "Evasion");
            _godMode = GUI.Toggle(new Rect(110f, 180f, 120f, 20f), _godMode, "God mode");
            _maxVueDistance = float.Parse(GUI.TextField(new Rect(110f, 200f, 120f, 20f), _maxVueDistance.ToString(), 10, "Distance"));

        }

        private double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2.0) + Math.Pow(y2 - y1, 2.0));
        }
    }
}