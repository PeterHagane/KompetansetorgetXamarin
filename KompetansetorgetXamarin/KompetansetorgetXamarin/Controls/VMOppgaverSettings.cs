using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.Controllers;
using System.Linq;
using KompetansetorgetXamarin.DAL;

namespace KompetansetorgetXamarin.Controls
{

    //in this class: 1. Get the filter lists from server. 2. Set the .Forms setting page to 2. Add all string names of the studyGroups fetched to a list. 3. 
    class VMOppgaverSettings : BaseContentPage  //only set to inherit from basecontentpage because of GoToLogin(); --- this is a fundamental design flaw on our part, as we're mixing two paradigms (MVVM and MVC). We're never going to use this as a page, but at least it works.
    {
        public ObservableCollection<fagområdeSetting> oppgaveSettings { get; set; }
        //private List<StudyGroup> fagområderList = null;
        private List<string> checkedStudyGroups = new List<string>();
        private List<string> names = new List<string>();

        public VMOppgaverSettings() {

            GetAllFilters();
            SetSettings();
            
            foreach (var fagområdeSetting in oppgaveSettings)
            {
                fagområdeSetting.OnToggled += ToggleSelection;
            }

        }



        void ToggleSelection(object sender, EventArgs e)
        {
            var fagområdeSetting = sender as fagområdeSetting;
            System.Diagnostics.Debug.WriteLine("{0} has been toggled to {1}", fagområdeSetting.Name, fagområdeSetting.IsSelected);
        }

        public List<string> GetSettings()
        {
            if (oppgaveSettings == null)
            {
                SetSettings();
                return checkedStudyGroups;
            }
            else
                foreach(fagområdeSetting setting in oppgaveSettings) {
                    bool checkSwitch = setting.IsSelected;
                    if (checkSwitch == true) {
                        checkedStudyGroups.Add(setting.Name);                
                    }
                }
                
            return checkedStudyGroups;
        }

        //TODO
        //Compare the full list from GetAllFilters and the smaller list from GetStudentFilter
        //add to oppgaveSettings(name, true) for every name in the list created by GetStudentFilter
        //add to oppgaveSettings(name, false) for every name not in the list created by GetStudentFilter
        //TODO
        public async void SetSettings() {
            
            if (oppgaveSettings == null)
            {
                oppgaveSettings = new ObservableCollection<fagområdeSetting> { };

                foreach (string name in names)
                {
                    oppgaveSettings.Add(new fagområdeSetting(name, true));
                }
            }
        }

        //Get the studygroups of the student, put the names into a list
        //specificList = studyGroupsFilter.Select(c => c.name).ToList();
        public async void GetStudentFilters() {
            //TODO
        }

        public async void GetAllFilters()//object sender, EventArgs e
        {
            DbLocation lc = new DbLocation();
            DbCourse cc = new DbCourse();
            DbStudyGroup sgc = new DbStudyGroup();
            DbJobType jtc = new DbJobType();
            List<Location> locationsFilter = lc.GetAllLocations();
            List<Course> coursesFilter = cc.GetAllCourses();
            List<StudyGroup> studyGroupsFilter = sgc.GetAllStudyGroups();
            List<JobType> jobTypesJobFilter = jtc.GetJobTypeFilterJob();
            List<JobType> jobTypesProjectFilter = jtc.GetJobTypeFilterProject();
            
            names = studyGroupsFilter.Select(c => c.name).ToList();

            //System.Diagnostics.Debug.WriteLine("GetAllFilters: locationsFilter.Count: " + locationsFilter.Count);
            //System.Diagnostics.Debug.WriteLine("GetAllFilters: coursesFilter.Count: " + coursesFilter.Count);
            //System.Diagnostics.Debug.WriteLine("GetAllFilters: studyGroupsFilter.Count: " + studyGroupsFilter.Count);
            //System.Diagnostics.Debug.WriteLine("GetAllFilters: jobTypesJobFilter.Count: " + jobTypesJobFilter.Count);
            //System.Diagnostics.Debug.WriteLine("GetAllFilters: jobTypesProjectFilter.Count: " + jobTypesProjectFilter.Count);
            //foreach (string id in ids)
            //{
            //    System.Diagnostics.Debug.WriteLine(id);
            //}

        }


    }
}

//OLD HARD CODED LIST
//    oppgaveSettings = new ObservableCollection<fagområdeSetting>
//  {
//    new fagområdeSetting("Administrasjon og ledelse",           true),
//   new fagområdeSetting("Datateknologi",                        true),
//    new fagområdeSetting("Helse- og sosialfag",                 true),
//  new fagområdeSetting("Historie, filosofi og religion",        true),
//    new fagområdeSetting("Idrettsfag",                          true),
//    new fagområdeSetting("Ingeniør- og teknologiske fag",       true),
//    new fagområdeSetting("Kunstfag",                            true),
//    new fagområdeSetting("Lærerutdanning og pedagogikk",        true),
//    new fagområdeSetting("Medie- og kommunikasjonsfag",         true),
//    new fagområdeSetting("Musikk",                              true),
//    new fagområdeSetting("Realfag",                             true),
//    new fagområdeSetting("Samfunnsfag",                         true),
//    new fagområdeSetting("Språk og litteratur",                 true),
//    new fagområdeSetting("Uspesifisert",                        true),
//    new fagområdeSetting("Økonomi og juss",                     true),
//};