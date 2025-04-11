namespace AE.Core.Systems.Audio
{
    public enum SoundType
    {
        None,

        // UI Sounds
        ButtonHover,
        ButtonClick,
        MenuOpen,
        MenuClose,

        // Gameplay Sounds
        PlayerFootstep,
        PlayerJump,
        ItemPickup,
        DoorOpen,
        DoorClose,

        // Combat Sounds
        WeaponFire,
        WeaponReload,
        Impact,

        // Ambience Sounds
        AmbientWind,
        AmbientCreak,

        // Feedback Sounds
        Success,
        Failure,
        Warning,

        // Add more as needed
    }
}