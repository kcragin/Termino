# Status

Pre-alpha

# Termino

Termino is a set of components used to manipulate the display and other
settings of Windows Terminal resources such as tabs, windows, and window panes.
The main use cases include, but aren't limited to, changing the visual effects
as the user navigates through a series of commands in Windows Terminal.

Sessions in the Windows Terminal CLI can be stateful in the sense that tools
such as `kubectl`, cloud providers such as AWS, and Azure, and other
stateful applications each provide some form of context.

Simply, it helps to know 'where you are' in a CLI experience and
'what you are doing'. Tasks such as port forwarding, working within a specific 
Kubernetes cluster, reading a log for a particular service, and so on
can benefit a user by orienting that person quickly and visually.

> Essentially, Termino is designed to provide visual cues to a human user,
based on what that user is doing.

# Conceptual Design and Experience

Termino has several distinct components that act in concert to provide these
visual clues. These components work at various levels of the user's contextual
navigation within a command line experience. The components include

- A core API that can manipulate Windows Terminal's visual experience from
any program. This API supports both synchronous and asynchronous commands that
are applied against the Windows Terminal user sessions.
- A CLI that can be used to set visual state from bash, PowerShell, and other popular shells
within a Windows Terminal session.
- A Windows Service that can be used to do the same, but in a way that is
out of process from the main CLI, or other application built against the
core API. The goal here, is to offload complex Windows Terminal visual state
changes from a calling library or application, ensuring the user's perceived
experience is responsive.

# Detailed Design


