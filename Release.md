# ChangeLog

**V2.7**

    ### Added
    - Advanced options panel for conversion tools
    - Expert options support for fine-tuning conversions
    - New Advanced and Expert UI icons/resources
    - Additional conversion settings for supported platforms

    ### Improved
    - Main converter UI layout
    - Conversion workflow organization
    - PS4 / PSVita / PS3 / PSP option handling
    - Conversion status messages
    - Error feedback using log-based details
    - Language files for updated UI/status text

    ### Changed
    - Cleaner status bar behavior
    - Shorter conversion error message
    - Updated project resources and designer files

**V2.6**

    - Unified the interface into a single screen, removing the separate AT9/AT3 tabs.
    - Added automatic detection for MP3, WAV, AT9, and AT3 files.
    - Added dynamic AT9/AT3 conversion selection.
    - Unified console and bitrate comboBox.
    - Added PSVita, PS4, PS3, and PSP icons with highlighting based on the selected console.
    - Fixed MP3/WAV conversion handling and existing-file checks.
    - Added a separate warning for existing intermediate WAV files.
    - Made tooltips and status messages translation-friendly.
    - Major code refactor to simplify maintenance and reduce conversion-related files/classes.

**V2.5**

    - Add **ATRAC9** Player support
    - Now you can read **AT9** (PSVita/PS4) and **AT3** (PSP/PS3) file final file or when you want to convert it :)
    - However, it's possible that not all AT9 or AT3 files are supported due to bitrate or other factors that need to be tested.
    - Add language support throughout the application

**V2.4**

    ## Changes by **@scorpio21**
    
        ### New Features
        - Added automatic audio resampling with NAudio.
        - Detects unsupported input sample rates automatically.
        - Strictly enforces `48000 Hz` for AT9 conversions.
        - Supports `44100 Hz` / `48000 Hz` for AT3 and PSP conversions.
        - Prevents Sony Tools `"Not Supported Param"` errors.
        - Added dynamic bitrate suffixes to converted filenames, for example `audio_96bit.at3`.
        - Allows testing multiple bitrates without overwriting previous outputs.
        - Added multi-language UI support.
        - Added a UI toggle to switch between Spanish and English.
        - Localized labels, tabs, buttons, and dynamic dialog messages.
        - Added centralized error logging to `conversion_errors.log`.
        - Replaced technical popups with a cleaner “Check log” status message.
        
        ### Bug Fixes & Stability
        - Fixed conversion failures when file paths contain spaces.
        - Properly quoted all arguments sent to external tools.
        - Fixed UAC/permission issues by using `UseShellExecute = false`.
        - Resolved Win32 Error `1223` caused by external process launches.
        - Replaced legacy `CodeBase` path handling.
        - Updated `MainForm_Load` and file movement logic to use `AppDomain.CurrentDomain.BaseDirectory`.
        - Fixed `"Path format not supported"` exceptions.
        - Forced strict `44100 Hz` stereo normalization for PSP conversions.
        - Improved PSP playback compatibility for files such as `SND0.AT3`.
        
        ### Technical Refactoring
        - Fixed NAudio references to use relative paths inside the repository.
        - Improved `MiniPlayer.exe` process management.
        - Added better tracking and termination of the MiniPlayer process.
        - Reduced file lock issues during recompilation.
        - Cleaned up unused code.
        - Removed unused variables such as `tip` and `tempFile`.
        - Reduced compiler warnings.
    
    ## Changes by **ME**
    
        - Add languages with comboBox
        - Improves little code
        - Rearranged the design

**V2.3**

    - Add PS4 Convertion
    - Add Player Actualy only for PSP AT3

**V2.2**

    - Add   BitRate support for AT9 and AT3
    - Add   Uppercase extention
    - Add   Compatibility PSP/PS3 for AT3
    - Add   Some verifications
    - Add   More compatibility for Sampling Rate 48000[kHz] and 44100[kHz]
    - Add   Status bar
    - Add   Info for AT9 and AT3 convertion 
    - Improved UI

**V2.1**

    - Add      Support 48000[kHz]  For At3tool
    - Add      Management Space in File
    - Remove   Browser Button
    - Add      Management of errors messages

**V2**

    - Add  Support for at3
    - Add  ToolTips
    - Add  Some verifications
    - Code Cleaned
    - UI   Improved

**V1.1**

    - Add   MP3 To at9
    - Add   at9 To MP3
    - Add   Drag&Drop
    - Add   Notifications
    - UI    Reformated

**V1.0**

    - Wav To At9
    - At9 To Wav
