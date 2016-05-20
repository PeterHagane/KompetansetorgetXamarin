using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.Controllers;
using System.Linq;
using KompetansetorgetXamarin.DAL;
using Xamarin.Forms.Xaml;


namespace KompetansetorgetXamarin.Controls
{
    //only set to inherit from basecontentpage because of GoToLogin(); 
    //--- this is a fundamental design flaw on our part, as we're mixing two paradigms (MVVM and MVC).
    //We're never going to use this as a page, but at least it works.

    class VMOppgaverSettings : BaseContentPage
    {
        public ObservableCollection<fagområdeSetting> oppgaveSettings { get; set; } //items in GUI
        public ObservableCollection<Location> locationsSettings = new ObservableCollection<Location>(); //items in GUI
        public ObservableCollection<Course> coursesSettings = new ObservableCollection<Course>(); //items in GUI

        private List<string> checkedStudyGroups = new List<string>();
        private List<StudyGroup> studyGroupsFilter = new List<StudyGroup>(); //gets used when retreiving projects/oppgaver in CarouselOppgaver

        public Dictionary<string, string> studyDict { private set; get; }
        public Dictionary<string, string> coursesFilter = new Dictionary<string, string>();  //gets used when retreiving projects/oppgaver in CarouselOppgaver



        public VMOppgaverSettings() {
            studyDict = new Dictionary<string, string>();

            GetAllFilters();
            SetSettings();
            
            foreach (var fagområdeSetting in oppgaveSettings)
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
            if (oppgaveSettings == null)
            {
                SetSettings();
                return null;
            }
            else
                foreach(fagområdeSetting setting in oppgaveSettings) {
                    bool checkSwitch = setting.IsSelected;
                    if (checkSwitch == true)
                    {
                        System.Diagnostics.Debug.WriteLine("setting.Name: " + setting.Name + ", id passed: " + setting.id);
                        checkedStudyGroups.Add(setting.id);                
                    }
                }
                
            return checkedStudyGroups;
        }

        public async void SetSettings()
        {

            if (oppgaveSettings == null)
            {
                oppgaveSettings = new ObservableCollection<fagområdeSetting> { };

                foreach (var sg in studyGroupsFilter)
                {
                    System.Diagnostics.Debug.WriteLine("string name in studyDict.Values: " + sg.name);
                    oppgaveSettings.Add(new fagområdeSetting(sg.name, sg.filterChecked, sg.id));
                }
            }
        }

        public void SaveSettings()
        {
            //DbLocation lc = new DbLocation();
            //DbCourse cc = new DbCourse();
            DbStudyGroup sgc = new DbStudyGroup();

            foreach (fagområdeSetting setting in oppgaveSettings)
            {
                //gets the name and setting from 
                string setName = setting.Name;
                bool setSwitch = setting.IsSelected;

                foreach (var studygroup in studyGroupsFilter)
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

        
        public async void GetStudentFilters() {
            //TODO
        }

        public async void GetAllFilters()//object sender, EventArgs e
        {
            //DbLocation lc = new DbLocation();
            //DbCourse cc = new DbCourse();
            DbStudyGroup sgc = new DbStudyGroup();
            //DbJobType jtc = new DbJobType();
            //List<Location> locationsGet = lc.GetAllLocations();
            //List<Course> coursesGet = cc.GetAllCourses();
            studyGroupsFilter = sgc.GetAllStudyGroups();
            //List<JobType> jobTypesJobGet = jtc.GetJobTypeFilterJob();
            //List<JobType> jobTypesProjectGet = jtc.GetJobTypeFilterProject();

            foreach (var studyGroup in studyGroupsFilter)
            {
                studyDict.Add(studyGroup.name, studyGroup.id);
            }

            //locationsFilter.Add(new Location(TODO)); //add empty location for default "nothing selected"
            //foreach (var location in locationsGet)
            //{
            //    locationsFilter.Add(location);
            //}
            //Course velgEmne = new Course();
            //velgEmne.name = "Velg emne";
            //coursesSettings.Add(velgEmne);
            //foreach (var course in coursesGet)
            //{
            //    coursesSettings.Add(course);
            //}

            // for jobs
            // DbLocation.UpdateLocations(List<Location>) sett max 1 til TRUE !
            // for projects
            // DbCourse.UpdateCourses(List<Course> courses) sett max 1 til TRUE !
            // for all
            // DbJobTypes.UpdateJobTypes(List<JobType> jobTypes) sett max 1 til TRUE !
            //
            // DbStudyGroup.UpdateStudyGroups(List<StudyGroup> studyGroups) 
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