# UI

A [Unity](http://unity3d.com/)-based UI for [Personal Food Computers](http://openag.media.mit.edu/hardware/).

## Builds

Up-to-date builds for Mac, Windows and Linux can be found in [grow-ui/App Builds/](https://github.com/OpenAgInitiative/gro-ui/tree/master/App%20Builds).

## Development

**Prerequisite**: You'll need [Unity](http://unity3d.com/) to develop the UI locally.

Unity requires project folders to follow a particular structure. Here's how to set up the repository files to follow that structure.

First, create a new directory. This directory will contain your Unity project.

    mkdir gro-ui-project

Next, check out the [gro-ui](https://github.com/OpenAgInitiative/gro-ui.git) repository to the `gro-ui-project/Assets` directory.

    cd gro-ui-project
    git checkout https://github.com/OpenAgInitiative/gro-ui.git Assets

Next, look for the ``gro-ui-project/Assets/ProjectSettings` directory. Copy this directory to the `gro-ui-project` directory.

Your `gro-ui-project` directory should now look like this:

    gro-ui-project/
      Assets/
      ProjectSettings/

Now, open Unity, click "open project" and select `grow-ui-project` from the dialog box.

Unity should load the project (this may take a while). When it's finished, a project window will open. You can now double-click the `_Scenes` folder and select one of the scenes.
