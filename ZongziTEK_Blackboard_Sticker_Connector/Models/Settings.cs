using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZongziTEK_Blackboard_Sticker_Connector.Models
{
    public class Settings : ObservableRecipient
    {
        private bool _isTimetableSyncEnabled = true;
        private bool _isClassIslandAutoHideEnabled = true;

        public bool IsTimetableSyncEnabled
        {
            get => _isTimetableSyncEnabled;
            set
            {
                if (value == _isTimetableSyncEnabled) return;
                _isTimetableSyncEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsClassIslandAutoHideEnabled
        {
            get => _isClassIslandAutoHideEnabled;
            set
            {
                if (value == _isClassIslandAutoHideEnabled) return;
                _isClassIslandAutoHideEnabled = value;
                OnPropertyChanged();
            }
        }
    }
}
