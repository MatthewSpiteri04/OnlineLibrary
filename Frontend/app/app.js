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
    .when('/security', {
        templateUrl:'views/security.html',
        controller: 'security-controller'
    })
    .when('/my-uploads', {
        templateUrl:'views/myUploads.html',
        controller: 'my-uploads-controller'
    })
    .when('/viewDocument/:id', {
        templateUrl:'views/viewDocument.html',
        controller: 'viewDocumentController'
    })
    .when('/categoryEditor/:id', {
        templateUrl:'views/categoryEditor.html',
        controller: 'categoryEditor-controller'
    })
    .otherwise({
        redirectTo: '/home'
    });
}]);

OnlineLibrary.service('userService', function($rootScope, $http) {
    var user = null;

    this.logout = function(){
        sessionStorage.clear()
        window.location.href = "#!/home";
        this.setCurrentUser(null);
    };

    this.getRoles = function(id) {
        return $http.get('https://localhost:44311/api/User/Roles/' + id)
        .then(response => {
            return response.data;
        })
    };
    
    this.getCurrentUser = function() {
        return JSON.parse(sessionStorage.getItem('loginData'));
    };

    this.setCurrentUser = function(newValue) {
        if(newValue != null) {
            user = newValue;
            this.getRoles(user.id).then(response => {
                user.Roles = response;
                sessionStorage.setItem('loginData', JSON.stringify(newValue));
                $rootScope.$broadcast('dataChanged', newValue);
            });
            console.log(newValue)
        }
        else {
            sessionStorage.setItem('loginData', JSON.stringify(null));
            $rootScope.$broadcast('dataChanged', newValue);
        }
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
    this.getFavourites = function(id, search) {
        var request = {
            userId: id,
            searchString: search
        }
        return $http.post('https://localhost:44311/api/Get/Favourites', request)
        .then(response => {
            return response.data;
        });
    };
});

OnlineLibrary.controller('favourites-controller', ['$scope', '$http', 'favouriteService', 'userService', '$uibModal', 'myUploadsService', 'uploadService', function($scope, $http, favouriteService, userService, $uibModal, myUploadsService, uploadService) {
    $scope.user = userService.getCurrentUser();
    $scope.filterOn = false;
    $scope.searchString = null;
    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });

    $scope.filteredItems = null;

    $scope.filterData = {
        category: null,
        author: null,
        language: null
    };

    if($scope.user != null) {
        favouriteService.getFavourites($scope.user.id, $scope.searchString)
        .then(data => { 
            $scope.filteredItems = data;
        });

        $scope.deleteDocument = function(document) {
            myUploadsService.deleteDocument(document).then(response => {
                favouriteService.getFavourites($scope.user.id, $scope.searchString)
                .then(data => { 
                    $scope.favourites = data;
                });
            });
        }

        $scope.submitFilterData = function() {
            $scope.searchForFavourites( $scope.searchString ).then(function(){
                $scope.favourites = angular.copy($scope.filteredItems);
                $scope.filteredItems = $scope.favourites.filter(function(document) {
                    var categoryMatch = !$scope.filterData.category || document.category === $scope.filterData.category;
                    var authorMatch = !$scope.filterData.author || document.author.toLowerCase().includes($scope.filterData.author.toLowerCase());                           
                    var languageMatch = !$scope.filterData.language || document.language === $scope.filterData.language;
                    return categoryMatch && authorMatch && languageMatch;
                });
                console.log($scope.filteredItems);
            });
        };

        $scope.viewDocument = function(document){
            console.log(document.id);
            window.location.href="#!/viewDocument/"+document.id;
        }

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
                if(reason == 'ok'){
                    var request = {
                        documentId: favourite.id,
                        userId: $scope.user.id,
                        isFavourite: favourite.isFavourite
                    };
                    $http.post('https://localhost:44311/api/Toggle/Favourite', request).then(function() {
                        favouriteService.getFavourites($scope.user.id)
                        .then(data => { 
                            $scope.filteredItems = data;
                        });    
                    });
                }
            }); 
        };

        $scope.downloadDocument = function(document){
            $http.post('https://localhost:44311/api/Download/Document', document, { responseType: 'arraybuffer' }).then(function(response) {
                var blob = new Blob([response.data]);
                var url = window.URL.createObjectURL(blob);
                
                // Create anchor element
                var a = angular.element('<a></a>');
                a.attr({
                    href: url,
                    download: document.title + document.fileExtension
                });
    
                // Append anchor element to document body
                angular.element(document.body).append(a);
    
                // Simulate click event
                a[0].click();
                
                // Clean up
                window.URL.revokeObjectURL(url);
                a.remove();
            })
            .catch(function(error) {
                console.error('Error downloading document:', error);
            });
        };

        $scope.searchForFavourites = function(searchString){
            var id = null;
            if($scope.user != null){
                var id = $scope.user.id;
            }

            return favouriteService.getFavourites(id, searchString)
            .then(data => {
                $scope.filteredItems = data;
            })
        };

    } else {
        window.location.href = "#!/home";
    }

    uploadService.getLanguages()
    .then(data => { 
        $scope.languages = data});
    uploadService.getCategories()
    .then(data => {
        $scope.categories = data});
}]);


OnlineLibrary.service('homeService', function($http) {
    this.getDocuments = function(request) {
        return $http.post('https://localhost:44311/api/Get/Documents', request)
        .then(response => {
            return response.data;
        });;
    };
});

  OnlineLibrary.controller('upload-controller', ['$scope', 'userService', 'uploadService', '$uibModal', 'categoryService', function($scope, userService, uploadService, $uibModal, categoryService){
    $scope.user = userService.getCurrentUser();
    $scope.publicAccess = false;
    $scope.attributes = [];
    $scope.categories = [];

    if($scope.user != null){ 
        if($scope.user.Roles.includes('Academic User')) {
            uploadService.getLanguages()
            .then(data => { 
                $scope.languages = data;
            });
            uploadService.getCategories()
            .then(data => {
                $scope.categories = data;
                console.log($scope.categories);
            });

            categoryService.getAccessLevels().then(response => {
                $scope.AccessLevels = response.data;
                console.log(response.data);
            });
            
            $scope.$on('dataChanged', function(event, data) {
                $scope.user = data;
            });

            function selectOption(value            ) {
                var selectElement = document.getElementById("mySelect");
                selectElement.selectedIndex = value;
              }

            $scope.getCategoryAccessLevels = function(id){
                $scope.categories.forEach(element => {
                    if(element.id == id) {
                        selectOption(element.publicAccess);
                    }
                });           
             }
        
            $scope.loadAttributes = function(id){
                $scope.getCategoryAccessLevels(id)
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
                console.log('pressed');
                var attributeList = [];
                const formData = new FormData(event.currentTarget);
        
                $scope.attributes.forEach(function(attr){
                    var temp = formData.get(attr.name);
                    attributeList.push({id: attr.id, value: temp});
                });
                event.preventDefault();
                formData.set("userId", $scope.user.id);
                formData.set("attributesListJSON", JSON.stringify(attributeList));
        
                fetch('https://localhost:44311/api/Upload/File', {
                    method: 'POST',
                    body: formData
                }).then(response =>{
                    if(response.status == 400){
                        $uibModal.open({
                            templateUrl: 'assets/elements/popup.html',
                            controller: 'popup-controller',
                            resolve: {
                                title: function(){
                                    return "File Not Supported";
                                },
                                message: function(){
                                    return "This file type is not recognised. Upload has been halted.";
                                }
                            }
                        }).result.then(function() { }, function(reason) {});
                    }
                    if (response.status == 200) {
                        $uibModal.open({
                            templateUrl: 'assets/elements/popup.html',
                            controller: 'popup-controller',
                            resolve: {
                                title: function(){
                                    return "Success";
                                },
                                message: function(){
                                    return "Your file has been successfully uploaded.";
                                }
                            }
                        }).result.then(function() { }, function(reason) {
                            location.reload();
                        });
                    }
                    window.location.href = "#!/home";
                })        
            }
        
        }
        else {
            window.location.href = '#!/home'
        }
    } else {
        window.location.href = '#!/home'
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

  OnlineLibrary.controller('home-controller', ['$scope', '$http', '$uibModal', 'homeService', 'userService', 'uploadService', 'myUploadsService', function($scope, $http, $uibModal,homeService, userService, uploadService, myUploadsService){
    $scope.user = userService.getCurrentUser();
    $scope.filterOn = false;
    $scope.documents = null;
    $scope.searchString = null;
    $scope.filteredItems = null;
    $scope.filterData = {
        category: null,
        author: null,
        language: null
    };

    $scope.viewDocument = function(document){
        window.location.href="#!/viewDocument/"+document.id;
    }

    $scope.submitFilterData = function() {
        $scope.searchForDocuments( $scope.searchString ).then(function(){
            $scope.documents = angular.copy($scope.filteredItems);
            $scope.filteredItems = $scope.documents.filter(function(document) {
                var categoryMatch = !$scope.filterData.category || document.category === $scope.filterData.category;
                var authorMatch = !$scope.filterData.author || document.author.toLowerCase().includes($scope.filterData.author.toLowerCase());                
                var languageMatch = !$scope.filterData.language || document.language === $scope.filterData.language;
                return categoryMatch && authorMatch && languageMatch;
            });
            console.log($scope.filteredItems);
        });
    };

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });

    $scope.downloadDocument = function(document){
        $http.post('https://localhost:44311/api/Download/Document', document, { responseType: 'arraybuffer' }).then(function(response) {
            var blob = new Blob([response.data]);
            var url = window.URL.createObjectURL(blob);
            
            // Create anchor element
            var a = angular.element('<a></a>');
            a.attr({
                href: url,
                download: document.title + document.fileExtension
            });

            // Append anchor element to document body
            angular.element(document.body).append(a);

            // Simulate click event
            a[0].click();
            
            // Clean up
            window.URL.revokeObjectURL(url);
            a.remove();
        })
        .catch(function(error) {
            console.error('Error downloading document:', error);
        });
    };

    $scope.deleteDocument = function(document) {
        myUploadsService.deleteDocument(document).then(response => {
            $scope.searchForDocuments($scope.searchString);
        });
    }

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
            $scope.filteredItems = data;
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
        })

    $scope.searching = function(search){
        $http.get('https://localhost:44311/api/help/' + search)
        .then(response => {
            $scope.helpDetails = response.data;
        })  
    }
  }]);

  OnlineLibrary.service('documentService', function($http) {
    this.updateDocument = function(document, id) {
        
        attr = [];

        document.attributes.forEach(element => {
            var x = angular.copy(element);
            x.value = x.value.toString();
            attr.push(x);
        });
        
        
        doc = document.document;

        docTemp = {
            document: doc,
            attributes: attr,
            userId: id
        };
        console.log(docTemp)

        return $http.put('https://localhost:44311/api/UpdateDocument', docTemp);
    };
});

  OnlineLibrary.controller('helpAnswerController', ['$scope', '$http', '$routeParams', function($scope, $http, $routeParams) {
    $scope.questionId = $routeParams.id;

    $http.get('https://localhost:44311/api/help/answer/' + $scope.questionId)
        .then(response => {
            $scope.helpDetails = response.data;
        })    
  }]);

  OnlineLibrary.controller('viewDocumentController', ['$scope', '$http', '$routeParams', 'userService', '$uibModal', 'uploadService', 'documentService', 'myUploadsService', function($scope, $http, $routeParams, userService, $uibModal, uploadService, documentService, myUploadsService) {
    $scope.documentId = $routeParams.id;
    $scope.user = userService.getCurrentUser();

    $scope.editMode = false;

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
        });

    $scope.loadDetails = function(){
        var id = 0
        if ($scope.user != null) {
            id = $scope.user.id
        }
        $http.get('https://localhost:44311/api/getDocument/' + $scope.documentId + '/' + id)
            .then(response => {
                $scope.Document = response.data;
                $scope.Document.attributes.forEach(element => {
                    if(element.tag == "number"){
                        element.value = parseInt(element.value);
                    }
                    if(element.tag == "date"){
                        element.value = new Date(element.value);
                    }
                });
                $scope.docUploadDate = new Date($scope.Document.document.uploadDate).toLocaleString();
                $scope.Document.attributes.forEach(element => {
                    if(element.value == 'on' && element.tag == 'checkbox'){
                        element.value = true;
                    }
                    else if (element.value != 'on' && element.tag == 'checkbox') {
                        element.value = false;
                    }
                });
                console.log($scope.Document);
        });
    }

    $scope.downloadItem = function(document) {
        $http.post('https://localhost:44311/api/Download/Document', document, { responseType: 'arraybuffer' }).then(function(response) {
            var blob = new Blob([response.data]);
            var url = window.URL.createObjectURL(blob);
            
            // Create anchor element
            var a = angular.element('<a></a>');
            a.attr({
                href: url,
                download: document.title + document.fileExtension
            });

            // Append anchor element to document body
            angular.element(document.body).append(a);

            // Simulate click event
            a[0].click();
            
            // Clean up
            window.URL.revokeObjectURL(url);
            a.remove();
        })
        .catch(function(error) {
            console.error('Error downloading document:', error);
        });
    }

    $scope.toggleEdit = function() {
        if($scope.user != null){
            if ($scope.editMode) {
                
                $scope.editMode = false;

                $scope.Document.attributes.forEach(element => {
                    if(element.value == true && element.tag == 'checkbox'){
                        element.value = 'on';
                    }
                    if (element.value == false && element.tag == 'checkbox') {
                        element.value = '';
                    }
                });

                documentService.updateDocument($scope.Document, $scope.user.id)
                .then(response => {
                    $scope.loadDetails();
                });
            }
            else {
                $scope.editMode = true;
            }
        }
        else {

        }
    }

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
                $scope.loadDetails();
            });
            
        }
    }

    $scope.deleteItem = function(){

        $uibModal.open({
            templateUrl: 'assets/elements/confirmation.html',
            controller: 'confirmation-controller',
            resolve: {
                title: function(){
                    return 'Deleting Document';
                },
                message: function(){
                    return 'Are you sure you want to delete this item?';
                }
            }
        }).result.then(function() { }, function(reason) { 
            if(reason == 'ok'){
                myUploadsService.deleteDocument($scope.Document.document).then(response => {
                    window.location.href = '#!/home'
                });
            }
        });

        
    }

    uploadService.getLanguages()
    .then(data => { 
        $scope.languages = data});

    $scope.loadDetails();

  }]);

  OnlineLibrary.controller('myInfo-controller', ['$scope', '$http', 'userService', function($scope, $http, userService){
    $scope.user = userService.getCurrentUser();

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
        });
    
    if($scope.user == null){ 
        window.location.href = '#!/home'
    }

    $scope.Logout = function(){
        userService.logout();
    }

    $scope.MyUploads = function(){
        window.location.href = "#!/my-uploads"
    }

    $scope.requestAcademicAccess = function() {
        var currentUser = userService.getCurrentUser();
        var userId = currentUser.id;
        var email = "chriscalleja04@gmail.com";
        var subject = "Request for Academic User Access";
        var message = "New request to grant academic user access has been requested from user with ID: " + userId + " . Grant at your own discretion";

        // Send HTTP request to API to send the email
        $http.post('https://localhost:44311/api/User/Send', { userId: userId, email: email, subject: subject, message: message })
            .then(function(response) {
                // Handle success
                console.log("Email sent successfully.");
            })
            .catch(function(error) {
                // Handle error
                console.error("Error sending email:", error);
            });
    };
  }]);

  // USERS CONTROLLER
  OnlineLibrary.controller('users-controller', ['$scope', '$http', 'userService', function($scope, $http, userService){
    $scope.currentUser = {};




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
                    userService.setCurrentUser(response.data);
                    $scope.currentUser = userService.getCurrentUser();
                    userService.getCurrentUser($scope.currentUser);
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
                    userService.setCurrentUser(response.data);
                    $scope.currentUser = userService.getCurrentUser();
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

OnlineLibrary.service('securityService', function($http) {
    this.updateAccountDetails = function(request) {
        return $http.put('https://localhost:44311/api/Update/UserInfo', request);
    };
    
    this.deleteAccount = function(id) {
        return $http.delete('https://localhost:44311/api/Delete/User/' + id);
    };
    this.deleteAccountAndDocuments = function(id) {
        return $http.delete('https://localhost:44311/api/Delete/UserDocuments/' + id);
    };
}); 

OnlineLibrary.controller('security-controller', ['$scope', 'userService', 'securityService', '$uibModal', function($scope, userService, securityService, $uibModal){
    $scope.user = userService.getCurrentUser();
    $scope.editMode = false;

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });
    
    if($scope.user != null) {
        $scope.userSecurity = angular.copy($scope.user);

        $scope.updateSecurity = function(){
            if($scope.editMode == false)
            {
                $scope.editMode = true;
            }
            else 
            {
                $scope.editMode = false;
                var request = {
                    id: $scope.user.id,
                    firstName: $scope.userSecurity.firstName,
                    lastName: $scope.userSecurity.lastName,
                    username: $scope.userSecurity.username,
                    email: $scope.userSecurity.email,
                    password: $scope.userSecurity.password,
                };

                securityService.updateAccountDetails(request)
                .then(response => {
                    userService.setCurrentUser(response.data);
                });
            }
        };

        $scope.deleteAccount = function(){
            $uibModal.open({
                templateUrl: 'assets/elements/deleteUserModal.html',
                controller: 'delete-user-modal-controller',
                resolve: {
                    title: function(){
                        return 'Deleting User';
                    },
                    message: function(){
                        return "Do you want to remove all documents you've uploaded?";
                    }
                }
            }).result.then(function() { }, function(reason) {
                if(reason == "yes"){
                    securityService.deleteAccountAndDocuments($scope.user.id)
                    .then(response => {
                        userService.logout();
                        $uibModal.open({
                            templateUrl: 'assets/elements/popup.html',
                            controller: 'popup-controller',
                            resolve: {
                                title: function(){
                                    return "User Account Deleted";
                                },
                                message: function(){
                                    return "User account has been removed successfully";
                                }
                            }
                        }).result.then(function() {}, function(reason) {});
                    }).catch(error => {
                        $uibModal.open({
                            templateUrl: 'assets/elements/popup.html',
                            controller: 'popup-controller',
                            resolve: {
                                title: function(){
                                    return error.data.title;
                                },
                                message: function(){
                                    return error.data.message;
                                }
                            }
                        }).result.then(function() {}, function(reason) {});
                    });
                }
                if(reason == "no") {
                    securityService.deleteAccount($scope.user.id)
                    .then(response => {
                        userService.logout();
                        $uibModal.open({
                            templateUrl: 'assets/elements/popup.html',
                            controller: 'popup-controller',
                            resolve: {
                                title: function(){
                                    return "User Account Deleted";
                                },
                                message: function(){
                                    return "User account has been removed successfully";
                                }
                            }
                        }).result.then(function() {}, function(reason) {});
                    }).catch(error => {
                        $uibModal.open({
                            templateUrl: 'assets/elements/popup.html',
                            controller: 'popup-controller',
                            resolve: {
                                title: function(){
                                    return error.data.title;
                                },
                                message: function(){
                                    return error.data.message;
                                }
                            }
                        }).result.then(function() {}, function(reason) {});
                    });
                }
            });




            
        };

    } else {
        window.location.href = "#!/home";
    };

}]);

OnlineLibrary.controller('delete-user-modal-controller', ['$scope', '$uibModalInstance', 'title', 'message', function($scope, $uibModalInstance, title, message){
    $scope.title = title;
    $scope.message = message;
    $scope.yes = function(){
        $uibModalInstance.dismiss('yes');
    }
    $scope.no = function(){
        $uibModalInstance.dismiss('no');
    }
  }]);

OnlineLibrary.service('myUploadsService', function($http) {
    this.getMyUploads = function(request) {
        return $http.post('https://localhost:44311/api/Get/MyUploads', request).then(response => {
            return response.data;
        });
    };

    this.deleteDocument = function(document){
        return $http.delete('https://localhost:44311/api/Delete/Document/' + document.id)
    }
}); 


OnlineLibrary.controller('my-uploads-controller', ['$scope', 'userService', 'myUploadsService', '$uibModal', '$http', 'uploadService', function($scope, userService, myUploadsService, $uibModal, $http, uploadService){
    $scope.user = userService.getCurrentUser();
    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });
    $scope.documents = []
    $scope.searchString = null;

    $scope.filterData = {
        category: null,
        language: null
    };

    $scope.filteredItems = null;

    if($scope.user != null) {

        $scope.deleteDocument = function(document) {
            myUploadsService.deleteDocument(document).then(response => {
                $scope.searchForDocuments($scope.searchString);
            });
        }

        $scope.viewDocument = function(document){
            console.log(document.id)
            window.location.href="#!/viewDocument/"+document.id;
        }

        $scope.submitFilterData = function() {
            $scope.searchForDocuments( $scope.searchString ).then(function(){
                $scope.documents = angular.copy($scope.filteredItems);
                $scope.filteredItems = $scope.documents.filter(function(document) {
                    var categoryMatch = !$scope.filterData.category || document.category === $scope.filterData.category;
                    var languageMatch = !$scope.filterData.language || document.language === $scope.filterData.language;
                    return categoryMatch && languageMatch;
                });
                console.log($scope.filteredItems);
            });
        };
        
        $scope.downloadDocument = function(document){
            $http.post('https://localhost:44311/api/Download/Document', document, { responseType: 'arraybuffer' }).then(function(response) {
                var blob = new Blob([response.data]);
                var url = window.URL.createObjectURL(blob);
                
                // Create anchor element
                var a = angular.element('<a></a>');
                a.attr({
                    href: url,
                    download: document.title + document.fileExtension
                });
    
                // Append anchor element to document body
                angular.element(document.body).append(a);
    
                // Simulate click event
                a[0].click();
                
                // Clean up
                window.URL.revokeObjectURL(url);
                a.remove();
            })
            .catch(function(error) {
                console.error('Error downloading document:', error);
            });
        };

        $scope.toggleFavourite = function(document) {
            var request = {
                documentId: document.id,
                userId: $scope.user.id,
                isFavourite: document.isFavourite
            };
            $http.post('https://localhost:44311/api/Toggle/Favourite', request).then(function() {
                $scope.searchForDocuments($scope.searchString);
            });
        }

        $scope.searchForDocuments = function(searchString){
            var response = {
                search: searchString,
                userId: $scope.user.id
            };
    
            return myUploadsService.getMyUploads(response)
            .then(data => {
                $scope.filteredItems = data;
            })
        };

        $scope.searchForDocuments($scope.searchString);

    }
    else{ 
        window.location.href = "#!/home";
    }

    uploadService.getLanguages()
    .then(data => { 
        $scope.languages = data});
    uploadService.getCategories()
    .then(data => {
        $scope.categories = data});
}]);

OnlineLibrary.controller('elements-controller', ['$scope', 'userService', '$uibModal', function($scope, userService, $uibModal){
    $scope.user = userService.getCurrentUser();

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });

    $scope.navigate = function(tab) {
        if(tab == 'help') {
            window.location.href = "#!/help";
        } else
        if ((tab == 'myinfo' || tab == 'favourites' || tab == 'security') && $scope.user == null) {
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
              }).result.then(function() {}, function(reason) {});
        }
        else if (tab == 'myinfo') {
            window.location.href = '#!/my-info';
        }
        else if (tab == 'favourites') {
            window.location.href = '#!/favourites';
        }
        else if (tab == 'security') {
            window.location.href = '#!/security';
        }
    }
}]);


OnlineLibrary.service('categoryService', function($http) {
    this.getAttributeTypes = function() {
        return $http.get('https://localhost:44311/api/Categories/AttributeTypes');
    };
    this.getAttributes = function(){
        return $http.get('https://localhost:44311/api/Categories/GetAttributes');
    };
    this.getAccessLevels = function() {
        return $http.get('https://localhost:44311/api/Get/AccessLevels');
    };
}); 

OnlineLibrary.controller('categories-controller', ['$scope', '$http', 'categoryService', 'userService', 'uploadService', '$uibModal', '$routeParams', function($scope, $http, categoryService, userService, uploadService, $uibModal, $routeParams){
    $scope.user = userService.getCurrentUser();
    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });

    $scope.screen = 'choice';

    $scope.AccessLevels = []
    $scope.attributeTypes = [];
    $scope.attributes = [];
    $scope.categories = [];

    $scope.tempItem = null;
    $scope.showList = false;
    
    if ($scope.user != null) {
        if($scope.user.Roles.includes('Manage Categories')) {
            $scope.changeScreen = function(value) {
                $scope.screen = value;
            }

            categoryService.getAccessLevels().then(response => {
                $scope.AccessLevels = response.data;
            });

            uploadService.getCategories()
            .then(data => {
                $scope.categories = data;
            });

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
                $scope.updateAttributes([]);
            })
            .catch(error => {
                console.error('Failed to fetch attributes:', error);
            });

            $scope.availableAttributes = [];
            $scope.inputFields = [];

            $scope.updateAttributes = function(inputs){
                var counter = 0;
                var temp = angular.copy($scope.attributes)
                var list = []
                var indexes = []
                inputs.forEach(element => {
                    if(element.Id) {
                        list.push(parseInt(element.Id));
                    }
                });
                temp.forEach(function(element, index) {
                    if(list.includes(element.id)){
                        indexes.push(index);
                    }
                });
                indexes.forEach(id => {
                    temp.splice(id - counter, 1);
                    counter += 1;
                });
                $scope.availableAttributes = angular.copy(temp);
            }
        
            $scope.toggleView = function(inputField) {
                // Toggle the listView property to switch between select and input views
                inputField.listView = !inputField.listView;
            
                // Clear the inputField.Name when switching to the input view
                if (!inputField.listView) {
                    inputField.Name = ''; // Clear the input field value
                }
            };

            $scope.deleteCategory = function(category, index) {
                $http.delete('https://localhost:44311/api/Delete/Category/'+ category.id).then(response => {
                    $scope.categories.splice(index, 1);
                }).catch(error => {
                    $uibModal.open({
                        templateUrl: 'assets/elements/popup.html',
                        controller: 'popup-controller',
                        resolve: {
                            title: function(){
                                return error.data.title;
                            },
                            message: function(){
                                return error.data.message;
                            }
                        }
                      }).result.then(function() {}, function(reason) {});
                });
            }

            $scope.editCategory = function(category){
                window.location.href='#!/categoryEditor/' + category.id
            }
            //HERE

            $scope.addInputField = function() {
                $scope.tempItem ={ 
                    Name: '',
                    listView: true,
                    placeholder: 'Select Type Name'
                };
            };

            $scope.assignToList = function(){
                console.log($scope.tempItem);
                $scope.inputFields.push($scope.tempItem);
                $scope.tempItem = null;
                $scope.updateAttributes($scope.inputFields);
            };
            
            $scope.removeInputField = function(index) {
                $scope.inputFields.splice(index, 1);
                $scope.updateAttributes($scope.inputFields);                
            };

            $scope.removeTempField = function() {
                $scope.tempItem = null;
            };

            $scope.changeType = function(temp) {
                $scope.attributes.forEach(element => {
                    if(element.id == temp.Id){
                        temp.placeholder = element.typeName;
                    }
                });
            };
        
        
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
                    AccessLevel: parseInt(categoryForm.accessLevel),
                    CategoryName: categoryForm.Name,
                    Attributes: inputFields,
                    UserId: id
                };

                console.log(categoryRequest);

                $http.post('https://localhost:44311/api/Categories/AddCategory', categoryRequest)
                    .then(function(response) {
                        if (response.status == 200) {
                            

                        

                    window.location.href = "#!/home";
                } 
            }) .catch(error => {console.log(error)});

            };
        } else {
            window.location.href = '#!/home';
        }
    } else {
        window.location.href = '#!/home';
    };
    
   
}]);


OnlineLibrary.service('categoryEditorService', function($http) {
    this.updateCategoryDetails = function(request) {
        return $http.put('https://localhost:44311/api/Update/Category', request);
    };
});


OnlineLibrary.controller('categoryEditor-controller', ['$scope', '$http', 'categoryService', 'categoryEditorService',  '$uibModal', '$routeParams', function($scope, $http, categoryService, categoryEditorService, $uibModal, $routeParams){
    $scope.categoryId = $routeParams.id;
    $scope.editMode = false;

    $scope.inputFields = [];
    $scope.attributeTypes = [];
    $scope.attributeList = [];

    $scope.showList = false;

    $scope.toggleShowList = function() {
        $scope.showList = !$scope.showList
    }

    $scope.updateAttributes = function(inputs){
        var counter = 0;
        var temp = angular.copy($scope.attributeList)
        var list = []
        var indexes = []
        inputs.forEach(element => {
            if(element.Id) {
                list.push(parseInt(element.Id));
            }
        });
        $scope.categoryResponse.attributes.forEach(element => {
            if(element.id) {
                list.push(parseInt(element.id));
            }
        });
       
        temp.forEach(function(element, index) {
            if(list.includes(element.id)){
                indexes.push(index);
            }
        });
        indexes.forEach(id => {
            temp.splice(id - counter, 1);
            counter += 1;
        });
        $scope.availableAttributes = angular.copy(temp);
        console.log($scope.availableAttributes);
    }

    $scope.tempItem = null;

    categoryService.getAttributeTypes()
    .then(response => {
        $scope.attributeTypes = response.data;
    })

    $scope.toggleView = function(inputField) {
        // Toggle the listView property to switch between select and input views
        inputField.listView = !inputField.listView;
    
        // Clear the inputField.Name when switching to the input view
        if (!inputField.listView) {
            inputField.Name = ''; // Clear the input field value
        }
    };

    $scope.changeType = function(temp) {
        $scope.availableAttributes.forEach(element => {
            if(element.id == temp.Id){
                temp.placeholder = element.typeName;
            }
        });
    };
    
    $scope.addInputField = function() {
        $scope.tempItem ={ 
            Name: '',
            listView: true,
            placeholder: 'Select Type Name'
        };
    };

    $scope.removeTempField = function() {
        $scope.tempItem = null;
    };

    $scope.removeInputFieldNew = function(index) {
        $scope.inputFields.splice(index, 1);
        $scope.updateAttributes($scope.inputFields);                
    };

    $scope.assignToList = function(){
        console.log($scope.tempItem);
        $scope.inputFields.push($scope.tempItem);
        $scope.tempItem = null;
        $scope.updateAttributes($scope.inputFields);
    };

    $scope.removeInputField = function(index) {
        $scope.categoryResponse.attributes.splice(index, 1);
        $scope.updateAttributes($scope.inputFields);                
    };

    $http.get('https://localhost:44311/api/Categories/GetCategories/' + $scope.categoryId)
    .then(response => {
        $scope.categoryResponse = response.data;
        console.log($scope.categoryResponse)

    }).then(function(){
        categoryService.getAttributes()
        .then(response => {
            $scope.attributeList = response.data;
            $scope.updateAttributes([]);
    })
    });

    $scope.updateCategory = function(){
        if($scope.editMode == false){
            $scope.editMode = true;
        }
        else{
            $scope.inputFields.forEach(element => {
                $scope.categoryResponse.attributes.push(element)
            });

            $scope.categoryResponse.attributes.forEach(element => {
                if(!element.listView && element.new){
                    element.TypeId = parseInt(element.TypeId)
                }
                if(element.listView && element.new){
                    element.Id = parseInt(element.Id)
                }
            });
            
            $scope.editMode = false;
            inputFields = [];
            tempItem = null;
            
           console.log($scope.categoryResponse.attributes);
            
            categoryEditorService.updateCategoryDetails($scope.categoryResponse)
            .then(response => {
                $scope.categoryResponse = response.data;
                console.log($scope.categoryResponse)
        
            });
        }
    }
    
    $scope.deleteCategory = function() {
        $http.delete('https://localhost:44311/api/Delete/Category/'+ $scope.categoryId).then(response => {
        window.location.href = "#!/catgory"
        }).catch(error => {
            $uibModal.open({
                templateUrl: 'assets/elements/popup.html',
                controller: 'popup-controller',
                resolve: {
                    title: function(){
                        return error.data.title;
                    },
                    message: function(){
                        return error.data.message;
                    }
                }
              }).result.then(function() {}, function(reason) {});
        });
    }

   
   
}]);
