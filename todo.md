


# Clear document hierarchy and separation of abstraction

Document referencing and information duplication between files should be minimized, the top readme is the starting point and it should reference one level down, 
for example only to docs/README.md and then docs readme should reference its children to minimize duplication of information 


The split between high level architecture and low level implementation is still very poor, this should be improved. THis is to improve readability and traceability of design and decisions taken.
Split up large files and create a folder hierarchy to enable fast navigation 

For example:
- Lift out a summery  of high level decisions in architecture/README.md and point to files in  architecture/ for information regarding the decisions

Split implementation into the following folders

design - design decisions
implementation - implementation plan and structure


Document the folder disposition and the purpose of each folder in docs/README.md


docs/plan.md should most likely be merged into implementation
docs/test-strategy.md most likely into testing
docs/design.md into a new ui folder, as well as parts should to into design and implementation

