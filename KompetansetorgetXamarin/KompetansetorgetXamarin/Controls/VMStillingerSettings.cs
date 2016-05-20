using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin.Controls
{
    class VMStillingerSettings : BaseContentPage
    {
        public ObservableCollection<fagområdeSetting> stillingerSettings { get; set; } //items in GUI
        public ObservableCollection<Location> locationsSettings = new ObservableCollection<Location>(); //items in GUI
        public ObservableCollection<Course> coursesSettings = new ObservableCollection<Course>(); //items in GUI

        private List<string> checkedStudyGroups = new List<string>();
        private List<StudyGroup> studyGroupsFilter = new List<StudyGroup>(); //gets used when retreiving projects/oppgaver in CarouselOppgaver

        public Dictionary<string, string> studyDict { private set; get; }
        public Dictionary<string, string> coursesFilter = new Dictionary<string, string>();


        public VMStillingerSettings()
        {
            studyDict = new Dictionary<string, string>();

            GetAllFilters();
            SetSettings();

            foreach (var fagområdeSetting in stillingerSettings)
            {
                fagområdeSetting.OnToggled += ToggleSelection;
            }

        }

        public void ToggleSelection(object sender, EventArgs e)
        {
            var fagområdeSetting = sender as fagområdeSetting;
            System.Diagnostics.Debug.WriteLine("{0} has been toggled to {1}", fagområdeSetting.Name, fagområdeSetting.IsSelected);
        }

        public List<string> GetSettings()
        {
            checkedStudyGroups.Clear();
            if (stillingerSettings == null)
            {
                SetSettings();
                return checkedStudyGroups; 
            }
            else
                foreach (fagområdeSetting setting in stillingerSettings)
                {
                    bool checkSwitch = setting.IsSelected;
                    if (checkSwitch == true)
                    {
                        System.Diagnostics.Debug.WriteLine("setting.Name: " + setting.Name);
                        checkedStudyGroups.Add(setting.id);
                    }
                }
            return checkedStudyGroups;
        }

        public async void SetSettings()
        {

            if (stillingerSettings == null)
            {
                stillingerSettings = new ObservableCollection<fagområdeSetting> { };

                foreach (var sg in studyGroupsFilter)
                {
                    System.Diagnostics.Debug.WriteLine("string name in studyDict.Values: " + sg.name);
                    stillingerSettings.Add(new fagområdeSetting(sg.name, sg.filterChecked, sg.id));
                }
            }
        }

        public void SaveSettings()
        {
            //DbLocation lc = new DbLocation();
            //DbCourse cc = new DbCourse();
            DbStudyGroup sgc = new DbStudyGroup();

            foreach (fagområdeSetting setting in stillingerSettings)
            {
                //gets the name and setting from 
                string setName = setting.Name;
                bool setSwitch = setting.IsSelected;

                foreach (StudyGroup studygroup in studyGroupsFilter)
                {
                    if (studygroup.name == setName)
                    {
                        studygroup.name = setName;
                        studygroup.filterChecked = setSwitch;
                        break;
                    }
                }
            }
            sgc.UpdateStudyGroups(studyGroupsFilter);
        }
        public async void GetAllFilters()
        {
            DbStudyGroup sgc = new DbStudyGroup();
            studyGroupsFilter = sgc.GetAllStudyGroups();
            
            foreach (var studyGroup in studyGroupsFilter)
            {
                studyDict.Add(studyGroup.name, studyGroup.id);
            }
        }


    }
}
