/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID PLAY_AMB_PAD_CASTLE = 1661945423U;
        static const AkUniqueID PLAY_AMB_PAD_FLOWER = 2459718302U;
        static const AkUniqueID PLAY_AMB_PAD_FOREST = 2928511412U;
        static const AkUniqueID PLAY_AMB_PAD_VILLAGE = 4196331637U;
        static const AkUniqueID PLAY_AMB_RFX_FORESTFAR = 2490489396U;
        static const AkUniqueID PLAY_AMB_RFX_FORESTMID = 2760358305U;
        static const AkUniqueID PLAY_FOL_FT_YULE_WALK = 2425283306U;
        static const AkUniqueID PLAY_MC_ATTACKHITENEMY = 1271315640U;
        static const AkUniqueID PLAY_MC_ATTACKS = 1852516592U;
        static const AkUniqueID PLAY_MC_FOOTSTEPS = 2368357634U;
        static const AkUniqueID PLAY_MC_PLAYERDIE = 1566142524U;
        static const AkUniqueID PLAY_MC_PRESENCES_ACTION = 2216081418U;
        static const AkUniqueID PLAY_MC_PRESENCES_WALK = 3206799055U;
        static const AkUniqueID PLAY_MC_ROLL = 1213444204U;
        static const AkUniqueID PLAY_MC_TAKEDAMAGE = 1096879491U;
        static const AkUniqueID PLAY_SFX_BELLRINGING = 2577559157U;
        static const AkUniqueID PLAY_SFX_YULE_ATTACKHITPLAYER = 3295576996U;
        static const AkUniqueID PLAY_SFX_YULE_ATTACKPOUNCE = 1923946266U;
        static const AkUniqueID PLAY_SFX_YULEATTACKCLAW = 1023339604U;
        static const AkUniqueID PLAY_UI_CLICKCONFIRMATION = 3498899374U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace STATE_AMB_PAD_ZONES
        {
            static const AkUniqueID GROUP = 1516422471U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID ST_AMB_PAD_CASTLE = 3490686090U;
                static const AkUniqueID ST_AMB_PAD_FLOWER = 1692906379U;
                static const AkUniqueID ST_AMB_PAD_FOREST = 2882467217U;
                static const AkUniqueID ST_AMB_PAD_VILLAGE = 1351658826U;
            } // namespace STATE
        } // namespace STATE_AMB_PAD_ZONES

    } // namespace STATES

    namespace SWITCHES
    {
        namespace SW_MC_FOOTSTEPSMATERIAL
        {
            static const AkUniqueID GROUP = 996959485U;

            namespace SWITCH
            {
                static const AkUniqueID SW_FT_CONCRETE = 916436602U;
                static const AkUniqueID SW_FT_DIRT = 2399563152U;
                static const AkUniqueID SW_FT_SNOW = 3850745462U;
            } // namespace SWITCH
        } // namespace SW_MC_FOOTSTEPSMATERIAL

        namespace SW_MC_FOOTSTEPSMOVEMENT
        {
            static const AkUniqueID GROUP = 2700451361U;

            namespace SWITCH
            {
                static const AkUniqueID SW_FT_ATTACK = 4253136415U;
                static const AkUniqueID SW_FT_ROLL = 1388451578U;
                static const AkUniqueID SW_FT_RUN = 2162921054U;
                static const AkUniqueID SW_FT_WALK = 303207424U;
            } // namespace SWITCH
        } // namespace SW_MC_FOOTSTEPSMOVEMENT

        namespace SW_MC_TYPEOFATTACKHIT
        {
            static const AkUniqueID GROUP = 4054447005U;

            namespace SWITCH
            {
                static const AkUniqueID SW_MC_ATTACKDISTANCEHIT = 1907146023U;
                static const AkUniqueID SW_MC_ATTACKMELEEHEAVYHIT = 3302953275U;
                static const AkUniqueID SW_MC_ATTACKMELEELIGHTHIT = 2734195228U;
            } // namespace SWITCH
        } // namespace SW_MC_TYPEOFATTACKHIT

        namespace SW_MC_TYPEOFENEMY
        {
            static const AkUniqueID GROUP = 2119059778U;

            namespace SWITCH
            {
                static const AkUniqueID SW_MC_HITKRAMPUS = 397687435U;
                static const AkUniqueID SW_MC_HITNAHAGAME = 2506029772U;
                static const AkUniqueID SW_MC_HITYULE = 295887543U;
            } // namespace SWITCH
        } // namespace SW_MC_TYPEOFENEMY

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID RTPC_VOLUMEMASTER = 424266135U;
        static const AkUniqueID RTPC_VOLUMEMUS = 3047961690U;
        static const AkUniqueID RTPC_VOLUMESFX = 3270307726U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID SB_GLOBAL = 2711878180U;
        static const AkUniqueID SB_ZONE1 = 2490956260U;
        static const AkUniqueID SB_ZONE2 = 2490956263U;
        static const AkUniqueID SB_ZONE3 = 2490956262U;
        static const AkUniqueID SB_ZONE4 = 2490956257U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMB = 1117531639U;
        static const AkUniqueID AMB_ZONE1_FOREST = 2724184479U;
        static const AkUniqueID AMB_ZONE2_FLOWER = 3135360100U;
        static const AkUniqueID AMB_ZONE3_VILLAGE = 470444002U;
        static const AkUniqueID AMB_ZONE4_CASTLE = 4210410935U;
        static const AkUniqueID ATTACKS_KRAMPUS = 738454910U;
        static const AkUniqueID ATTACKS_NAHAGAME = 1552412047U;
        static const AkUniqueID ATTACKS_YULE = 327602828U;
        static const AkUniqueID BOSS = 1560169506U;
        static const AkUniqueID FOL_MC_FOOTSTEPS = 3692117971U;
        static const AkUniqueID FOL_MC_PRESENCES = 1773447876U;
        static const AkUniqueID FOL_MC_ROLL = 2268577727U;
        static const AkUniqueID FT_KRAMPUS = 1566882829U;
        static const AkUniqueID FT_NAHAGAME = 361833422U;
        static const AkUniqueID FT_YULE = 1342630121U;
        static const AkUniqueID KRAMPUS = 496251272U;
        static const AkUniqueID MAIN_AUDIO_BUS = 2246998526U;
        static const AkUniqueID MASTER_MUS = 3944324759U;
        static const AkUniqueID MASTER_SD = 1202187523U;
        static const AkUniqueID MC = 1685527061U;
        static const AkUniqueID NAHAGAME = 786071385U;
        static const AkUniqueID PRES_KRAMPUS = 2285774913U;
        static const AkUniqueID PRES_NAHAGAME = 2749025162U;
        static const AkUniqueID PRES_YULE = 4114299389U;
        static const AkUniqueID REVERBS = 3545700988U;
        static const AkUniqueID SFX = 393239870U;
        static const AkUniqueID SFX_MC_ATTACK = 854716996U;
        static const AkUniqueID SFX_MC_ATTACKHITENNEMY = 1128311319U;
        static const AkUniqueID SFX_MC_PLAYER_TAKEDAMAGE = 1743438368U;
        static const AkUniqueID UI = 1551306167U;
        static const AkUniqueID YULE = 2154073102U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID REV_CASTLE = 1216683595U;
        static const AkUniqueID REV_FLOWER = 1574958010U;
        static const AkUniqueID REV_FOREST = 3910197480U;
        static const AkUniqueID REV_VILLAGE = 4120165417U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
