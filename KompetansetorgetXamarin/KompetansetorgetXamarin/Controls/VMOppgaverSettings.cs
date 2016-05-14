﻿using System;
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
        public Dictionary<string, string> studyDict { private set; get; }
        private List<StudyGroup> studyGroupsFilter = new List<StudyGroup>();




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
                        System.Diagnostics.Debug.WriteLine("setting.Name: " + setting.Name);
                        checkedStudyGroups.Add(studyDict[setting.Name]);                
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

                foreach (var sg in studyGroupsFilter)
                {
                    System.Diagnostics.Debug.WriteLine("string name in studyDict.Values: " + sg.name);
                    oppgaveSettings.Add(new fagområdeSetting(sg.name, sg.filterChecked));
                }
            }
        }

        //public async void SetSettings()
        //{

        //    if (oppgaveSettings == null)
        //    {
        //        oppgaveSettings = new ObservableCollection<fagområdeSetting> { };

        //        foreach (string name in studyDict.Keys)
        //        {
        //            System.Diagnostics.Debug.WriteLine("string name in studyDict.Values: " + name);
        //            oppgaveSettings.Add(new fagområdeSetting(name, true));
        //        }
        //    }
        //}


        public void SaveSettings()
        {
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
            studyGroupsFilter = sgc.GetAllStudyGroups();
            List<JobType> jobTypesJobFilter = jtc.GetJobTypeFilterJob();
            List<JobType> jobTypesProjectFilter = jtc.GetJobTypeFilterProject();


            foreach (var studygroup in studyGroupsFilter)
            {
                studyDict.Add(studygroup.name, studygroup.id);
            }

            // for jobs
            // DbLocation.UpdateLocations(List<Location>) sett max 1 til TRUE !
            // for projects
            // DbCourse.UpdateCourses(List<Course> courses) sett max 1 til TRUE !
            // for all
            // DbJobTypes.UpdateJobTypes(List<JobType> jobTypes) sett max 1 til TRUE !
            //
            // DbStudyGroup.UpdateStudyGroups(List<StudyGroup> studyGroups) 


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