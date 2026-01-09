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
        private bool _isTimetableShared = true;
        private bool _isClassIslandAutoHideEnabled = true;

        public bool IsTimetableShared
        {
            get => _isTimetableShared;
            set
            {
                if (value == _isTimetableShared) return;
                _isTimetableShared = value;
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
