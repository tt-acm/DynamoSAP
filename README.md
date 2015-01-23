DynamoSAP
=========

DynamoSAP is a parametric interface for [SAP2000](http://www.csiamerica.com/products/sap2000), built on top of [Dynamo](http://dynamobim.org/).  The project enables designers and engineers to generatively author and analyze structural systems in SAP, using Dynamo to drive the SAP model.  The project prescribes a few common workflows which are described in the included sample files, and provides a wide range of opportunities for automation of typical tasks in SAP.

DynamoSAP is being developed in C# using Visual Studio, and has been tested with Dynamo 0.7.5 and SAP200 version 16.  The project consists of three libraries; [DynamoSAP](https://github.com/tt-acm/DynamoSAP/tree/master/src/DynamoSAP) contains the Dynamo [zero-touch](https://github.com/DynamoDS/Dynamo/wiki/Zero-Touch-Plugin-Development) nodes, [DynamoSAP_UI](https://github.com/tt-acm/DynamoSAP/tree/master/src/DynamoSAP_UI) contains nodes with custom user interfaces (dropdowns for example), and [SAPConnection](https://github.com/tt-acm/DynamoSAP/tree/master/src/SAPConnection) handles interacting with the SAP API.

The [DynamoSAP](https://github.com/tt-acm/DynamoSAP/tree/master/src/DynamoSAP) library is a generic representation of a structural model for use in Dynamo - it is in no way tied to SAP2000’s API.  This is part of the rationale behind having separate projects for the core structural definitions (DynamoSAP) and the SAP2000 specific classes (SAPConnection).  We are hopeful that this project can be easily forked and modified to make a similar connection to another structural solver.


We are developing nodes in four main categories:

- Structure.  These nodes describe building elements in a structural model - Frames and Shells.

- Definitions.  These nodes describe entities in the SAP model that aren’t building elements - things like material and section properties, loads and load patterns, load cases and combinations, releases, restraints and groups.  

- Assembly.  These nodes allow for the composition and decomposition of entire structural models, and for the reading and writing of data to and from SAP.  

- Analysis.  These nodes relate to running analyses in SAP and retrieving analysis results from SAP for visualization (or any other use) in Dynamo.


DynamoSAP is developed and maintained by Thornton Tomasetti’s CORE studio.  The main developers are:

- [Elcin Ertugrul](https://github.com/eertugrul)
- [Ana Gracia-Puyol](https://github.com/anagpuyol)
