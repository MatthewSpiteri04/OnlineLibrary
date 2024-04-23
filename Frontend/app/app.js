var OnlineLibrary = angular.module('OnlineLibrary', ['ngRoute']);

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
        controller: 'users-controller'
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
    .when('/category', {
        templateUrl:'views/category.html',
        controller: 'categories-controller'
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

  OnlineLibrary.controller('upload-controller', ['$scope', 'userService', 'uploadService', function($scope, userService, uploadService){
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
        
        
        formData.set("publicAccess", $scope.publicAccess);
        formData.set("userId", $scope.user.id);
        formData.set("attributesListJSON", JSON.stringify(attributeList));
        console.log(formData.get("attributesListJSON"));

        fetch('https://localhost:44311/api/Upload/File', {
            method: 'POST',
            body: formData
        }).then(response =>{
            console.log(response);
        });

    }

  }]);


  OnlineLibrary.controller('home-controller', ['$scope', '$http', 'userService', 'uploadService', function($scope, $http, userService, uploadService){
    $scope.user = null;
    $scope.filterOn = false;

    uploadService.getLanguages()
    .then(data => { 
        $scope.languages = data});
    uploadService.getCategories()
    .then(data => {
        $scope.categories = data});

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });


  }]);

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
    };
    
    $scope.removeInputField = function(index) {
        $scope.inputFields.splice(index, 1);
    };

   
   
    $scope.createCategoryAndAttributes = function(categoryForm, inputFields) {
        var categoryRequest = {
            CategoryName: categoryForm.Name,
            Attributes: inputFields,
            UserId: $scope.user.id
        };
       
         console.log(categoryRequest);
        

        $http.post('https://localhost:44311/api/Categories/AddCategory', categoryRequest)
            .then(function(response) {
                if (response.status == 200) {
                    

                

            window.location.href = "#!/home";
        } 
    }) .catch(error => {console.log(error)});

    };

}]);
