# .NET 6 Scheduler

Created using .NET6 and Quartz, this application demonstrates the use of Quartz on .NET 6 application by creating a scheduler every 1 minute from 9:00 to 6:00 (office hours). The scheduler collects bills from Database (SQL server) and pushes to different server every 1 minute. 
This is actually just an example and you can customize it anyway you like.

## Installation

You might have to create your own DB as no initialization/seeding is done. 


## Features

* Made using .NET 6 and Quartz 3.4.0.
* All setting setup at appsettings.json file. You can configure your own at appsettings. 
* Added QuartzConfiguratorExtensions where you can configure dynamic jobs as you like. 

## Additional Features
- .NET 6 clean architecture implementation. 
- Dynamic DI implementation. Yes, you don't need to worry on adding Iservices every time you create a new one. :) 



## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
