var OnlineLibrary = angular.module('OnlineLibrary', ['ngRoute', 'ui.bootstrap', 'ngAnimate']);

OnlineLibrary.config(['$routeProvider', function($routeProvider) {
    $routeProvider
    .when('/home', {
        templateUrl:'views/home.html',
        controller: 'home-controller'
    })
    .when('/login', {
        templateUrl:'views/login.html',
        controller: 'users-controller'
    })
    .when('/sign-up', {
        templateUrl:'views/sign-up.html',
        controller: 'users-controller'
    })
    .when('/help', {
        templateUrl:'views/help.html',
        controller: 'help-controller'
    })
    .when('/helpAnswer/:id', {
        templateUrl:'views/helpAnswer.html',
        controller: 'helpAnswerController'
    })
    .when('/upload', {
        templateUrl:'views/upload.html',
        controller: 'upload-controller'
    })
    .when('/my-info', {
        templateUrl:'views/my-info.html',
        controller: 'myInfo-controller'
    })
    .when('/category', {
        templateUrl:'views/category.html',
        controller: 'categories-controller'
    })
    .when('/favourites', {
        templateUrl:'views/favourites.html',
        controller: 'favourites-controller'
    })
    .otherwise({
        redirectTo: '/home'
    });
}]);

OnlineLibrary.service('userService', function($rootScope, $http) {
    var user = null;
    
    this.getCurrentUser = function() {
        return user;
    };

    this.setMyVariable = function(newValue) {
        user = newValue;
        $rootScope.$broadcast('dataChanged', newValue);
    };
  });

OnlineLibrary.service('uploadService', function($rootScope, $http) {
    this.getLanguages = function(){
        return $http.get('https://localhost:44311/api/Get/Languages')
        .then(response => {
            return data = response.data;
        });
    };

    this.getCategories = function() {
        return $http.get('https://localhost:44311/api/Get/Categories')
        .then(response => {
            return response.data;
        });
    };

    this.getAttributes = function(categoryId) {
        return $http.get('https://localhost:44311/api/Get/Attributes/' + categoryId)
        .then(response => {
            return response.data;
        });
    };
  });

  OnlineLibrary.service('favouriteService', function($http) {
    this.getFavourites = function(id) {
        return $http.get('https://localhost:44311/api/Get/Favourites/' + id)
        .then(response => {
            return response.data;
        });
    };
});

OnlineLibrary.controller('favourites-controller', ['$scope', '$http', 'favouriteService', 'userService', '$uibModal', function($scope, $http, favouriteService, userService, $uibModal) {
    $scope.user = userService.getCurrentUser();
    // $scope.filterOn = false;
    // $scope.documents = null;
    // $scope.searchString = null;
    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });

    favouriteService.getFavourites($scope.user.id)
    .then(data => { 
        $scope.favourites = data;
    });

    $scope.toggleFavourite = function(favourite, i){
        $uibModal.open({
            templateUrl: 'assets/elements/confirmation.html',
            controller: 'confirmation-controller',
            resolve: {
                title: function(){
                    return 'Removing from Favourites';
                },
                message: function(){
                    return 'Are you sure you want to remove this item from favourites';
                }
            }
          }).result.then(function() { }, function(reason) {
            if(reason == 'ok') {
                $uibModal.open({
                    templateUrl: 'assets/elements/popup.html',
                    controller: 'popup-controller',
                    resolve: {
                        title: function(){
                            return 'Successfully Removed';
                        },
                        message: function(){
                            return 'Your item has been successfully removed';
                        }
                    }
                  }).result.then(function() { }, function(reason) {});

                var request = {
                    documentId: favourite.id,
                    userId: $scope.user.id,
                    isFavourite: favourite.isFavourite
                };
                $http.post('https://localhost:44311/api/Toggle/Favourite', request).then(function() {
                    favouriteService.getFavourites($scope.user.id)
                    .then(data => { 
                        $scope.favourites = data;
                    });    
                });
            }
          }); 

        
    };

    
    // $scope.searchForFavourites = function(searchString){
    //     var id = null;
    //     if($scope.user != null){
    //         var id = $scope.user.id;
    //     }

    //     var response = {
    //         search: searchString,
    //         userId: id
    //     };

    //     return favouriteService.getFavourites(response)
    //     .then(data => {
    //         $scope.favourites = data;
    //     })
    // };

    // $scope.toggleFavourite = function(favourites) {
    //     if($scope.user == null || $scope.user == ''){
    //         $uibModal.open({
    //             templateUrl: 'assets/elements/popup.html',
    //             controller: 'popup-controller',
    //             resolve: {
    //                 title: function(){
    //                     return 'User Not Logged In';
    //                 },
    //                 message: function(){
    //                     return 'This feature requires the user to be logged in.';
    //                 }
    //             }
    //           }).result.then(function() {}, function(reason) {}); // Handling the modal return
    //     } else {
    //         var request = {
    //             favouritesId: favourites.id,
    //             userId: $scope.user.id,
    //             isFavourite: favourites.isFavourite
    //         };
    //         $http.post('https://localhost:44311/api/Toggle/Favourite', request).then(function() {
    //             $scope.searchForFavourites($scope.searchString);
               
    //         });
            
    //     }
       
    // }


}]);


OnlineLibrary.service('homeService', function($http) {
    this.getDocuments = function(request) {
        return $http.post('https://localhost:44311/api/Get/Documents', request)
        .then(response => {
            return response.data;
        });;
    };
});

  OnlineLibrary.controller('upload-controller', ['$scope', 'userService', 'uploadService', '$uibModal', function($scope, userService, uploadService, $uibModal){
    $scope.user = userService.getCurrentUser();
    $scope.publicAccess = false;
    $scope.attributes = [];
    $scope.categories = [];
    
    uploadService.getLanguages()
    .then(data => { 
        $scope.languages = data;
    });
    uploadService.getCategories()
    .then(data => {
        $scope.categories = data;
    });
    
    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });

    $scope.loadAttributes = function(id){
        uploadService.getAttributes(id)
        .then(data => {
            $scope.attributes = data;
        });
        $scope.categories.forEach(element => {
            if(element.id == id){
                $scope.categoryName = element.type;
            }
        });
    };

    const form = document.querySelector('form');
    if (!form) return;
    form.addEventListener('submit', handleSubmit);

    function handleSubmit(event) {
        var attributeList = [];
        const formData = new FormData(event.currentTarget);

        $scope.attributes.forEach(function(attr){
            var temp = formData.get(attr.name);
            attributeList.push({id: attr.id, value: temp});
        });
        event.preventDefault();
        console.log(event);
        debugger;
        
        formData.set("publicAccess", $scope.publicAccess);
        formData.set("userId", $scope.user.id);
        formData.set("attributesListJSON", JSON.stringify(attributeList));
        console.log(formData.get("attributesListJSON"));

        fetch('https://localhost:44311/api/Upload/File', {
            method: 'POST',
            body: formData
        }).then(response =>{
            
            window.location.href = "#!/home"
        });

    }

  }]);

  OnlineLibrary.controller('popup-controller', ['$scope', '$uibModalInstance', 'title', 'message', function($scope, $uibModalInstance, title, message){
    $scope.title = title;
    $scope.message = message;
    $scope.closePopup = function(){
        $uibModalInstance.dismiss('close');
    }
  }]);

  OnlineLibrary.controller('confirmation-controller', ['$scope', '$uibModalInstance', 'title', 'message', function($scope, $uibModalInstance, title, message){
    $scope.title = title;
    $scope.message = message;
    $scope.cancel = function(){
        $uibModalInstance.dismiss('close');
    }
    $scope.ok = function(){
        $uibModalInstance.dismiss('ok');
    }
  }]);

  OnlineLibrary.controller('home-controller', ['$scope', '$http', '$uibModal', 'homeService', 'userService', 'uploadService', function($scope, $http, $uibModal,homeService, userService, uploadService){
    $scope.user = userService.getCurrentUser();
    $scope.filterOn = false;
    $scope.documents = null;
    $scope.searchString = null;

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });

    $scope.toggleFavourite = function(document) {
        if($scope.user == null || $scope.user == ''){
            $uibModal.open({
                templateUrl: 'assets/elements/popup.html',
                controller: 'popup-controller',
                resolve: {
                    title: function(){
                        return 'User Not Logged In';
                    },
                    message: function(){
                        return 'This feature requires the user to be logged in.';
                    }
                }
              }).result.then(function() {}, function(reason) {}); // Handling the modal return
        } else {
            var request = {
                documentId: document.id,
                userId: $scope.user.id,
                isFavourite: document.isFavourite
            };
            $http.post('https://localhost:44311/api/Toggle/Favourite', request).then(function() {
                $scope.searchForDocuments($scope.searchString);
            });
            
        }
    }
    
    $scope.searchForDocuments = function(searchString){
        var id = null;
        if($scope.user != null){
            var id = $scope.user.id;
        }

        var response = {
            search: searchString,
            userId: id
        };

        return homeService.getDocuments(response)
        .then(data => {
            $scope.documents = data;
        })
    };

    uploadService.getLanguages()
    .then(data => { 
        $scope.languages = data});
    uploadService.getCategories()
    .then(data => {
        $scope.categories = data});

    


  }]);


  OnlineLibrary.controller('help-controller', ['$scope', '$http', 'userService', function($scope, $http, userService){

    $http.get('https://localhost:44311/api/help')
        .then(response => {
            $scope.helpDetails = response.data;
            console.log($scope.helpDetails);
        })

    $scope.searching = function(search){
    console.log(search);
        $http.get('https://localhost:44311/api/help/' + search)
        .then(response => {
            $scope.helpDetails = response.data;
            console.log($scope.helpDetails);
        })  
    }
  }]);


  /*----------------------------------------*/
  OnlineLibrary.controller('helpAnswerController', ['$scope', '$http', '$routeParams', function($scope, $http, $routeParams) {
    $scope.questionId = $routeParams.id;

    $http.get('https://localhost:44311/api/help/answer/' + $scope.questionId)
        .then(response => {
            $scope.helpDetails = response.data;
            console.log($scope.helpDetails);
        })
    
  }]);
  /*---------------------------------------*/

  OnlineLibrary.controller('myInfo-controller', ['$scope', '$http', 'userService', function($scope, $http, userService){
    $scope.user = userService.getCurrentUser();

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
        console.log($scope.user);
    });


  }]);

  // USERS CONTROLLER
  OnlineLibrary.controller('users-controller', ['$scope', '$http', 'userService', function($scope, $http, userService){
    $scope.currentUser = {};

    $scope.getRoles = function(id) {
        $http.get('https://localhost:44311/api/User/Roles/' + id)
            .then(response => {
                $scope.currentUser.Roles = response.data;
            })
    }

   

    $scope.doesUserExist = function(loginForm) {
        $scope.userFound = false;
        $http.get('https://localhost:44311/api/User/Exists/'+ loginForm.login)
            .then(response => {
                $scope.login(response.data, loginForm);
            })
    };

    $scope.login = function(userFound, loginForm) {
        if (userFound) {
            $http.post('https://localhost:44311/api/User/Login', {login: loginForm.login, password: loginForm.password})
            .then(response => {
                if (response.status == 200) {
                    $scope.currentUser = response.data;
                    $scope.getRoles(response.data.id);
                    userService.setMyVariable($scope.currentUser);
                    window.location.href = "#!/home";
                }
                else {
                    console.log('NOT FOUND');
                }
                
            });
        }
        else {
            console.log('NOT FOUND');
        };
    };

    $scope.signUpUser = function(signUpForm) {
        request = {
            firstName: signUpForm.firstName,
            lastName: signUpForm.lastName,
            email: signUpForm.email,
            username: signUpForm.username,
            password: signUpForm.password,
            passwordConfirmation: signUpForm.passwordConfirmation
        }

        if (request.password == request.passwordConfirmation) {
            $http.post('https://localhost:44311/api/User/Add', request)
            .then(response => {
                if (response.status == 200) {
                    $scope.currentUser = response.data;
                    $scope.getRoles(response.data.id);
                    console.log($scope.currentUser);
                    window.location.href = "#!/home";
                }
                else {
                    console.log('FAILED TO CREATE USER');
                }
            });
        }
        else {
            console.log("PASSWORDS DON'T MATCH");
        }
    };

   
    $scope.togglePassword = function() {
        $scope.typePassword = !$scope.typePassword;
        
      };

    $scope.toggleConfirmPassword = function() {
        $scope.typeConfirmPassword = !$scope.typeConfirmPassword;
       
      };

  
}]);

OnlineLibrary.controller('elements-controller', ['$scope', 'userService', function($scope, userService){
    $scope.user = userService.getCurrentUser();

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
        console.log($scope.user);
    });
}]);


OnlineLibrary.service('categoryService', function($http) {
    this.getAttributeTypes = function() {
        return $http.get('https://localhost:44311/api/Categories/AttributeTypes');
    };
    this.getAttributes = function(){
        return $http.get('https://localhost:44311/api/Categories/GetAttributes');
    };
}); 

OnlineLibrary.controller('categories-controller', ['$scope', '$http', 'categoryService', 'userService', function($scope, $http, categoryService, userService){
    $scope.user = userService.getCurrentUser();
    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });

    $scope.attributeTypes = [];
    $scope.attributes = [];
    
    categoryService.getAttributeTypes()
        .then(response => {
            $scope.attributeTypes = response.data;
        })
        .catch(error => {
            console.error('Failed to fetch attribute types:', error);
        });

        categoryService.getAttributes()
        .then(response => {
            
            $scope.attributes = response.data;
            console.log($scope.attributes);
        })
        .catch(error => {
            console.error('Failed to fetch attributes:', error);
        });

    
    $scope.inputFields = [];

  
    $scope.toggleView = function(inputField) {
        // Toggle the listView property to switch between select and input views
        inputField.listView = !inputField.listView;
    
        // Clear the inputField.Name when switching to the input view
        if (!inputField.listView) {
            inputField.Name = ''; // Clear the input field value
        }
    };
    

      $scope.addInputField = function() {
        $scope.inputFields.push({ 
            Name: '',
            listView: true 
        });
        console.log($scope.inputFields);
    };
    
    $scope.removeInputField = function(index) {
        $scope.inputFields.splice(index, 1);
    };

    $scope.changeOption = function(id, type){
        id.TypeId = parseInt(id.TypeId);
        console.log(id);
        console.log(type);
        console.log($scope.attributeTypes)
    }

   
   
    $scope.createCategoryAndAttributes = function(categoryForm, inputFields) {
        inputFields.forEach(element => {
            if(!element.listView){
                element.TypeId = parseInt(element.TypeId)
            }
         });
         
         var id = 0;
        if ($scope.user == null) {
            id = 3;
        }
        else{
            id = $scope.user.id;
        }
        
        var categoryRequest = {
            CategoryName: categoryForm.Name,
            Attributes: inputFields,
            UserId: id
        };
        console.log($scope.attributeTypes);
         console.log(categoryRequest);
        

        $http.post('https://localhost:44311/api/Categories/AddCategory', categoryRequest)
            .then(function(response) {
                if (response.status == 200) {
                    

                

            window.location.href = "#!/home";
        } 
    }) .catch(error => {console.log(error)});

    };

}]);
