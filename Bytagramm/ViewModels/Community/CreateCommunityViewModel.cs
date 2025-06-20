﻿using Bytagramm.Dto.Community;
using Bytagramm.Services.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bytagramm.ViewModels.Community
{
    public partial class CreateCommunityViewModel : ObservableObject
    {
        private readonly ICommunityApiService _communityApiService;

        public CreateCommunityViewModel(ICommunityApiService communityApiService)
        {
            _communityApiService = communityApiService;
        }

        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public string description;

        [RelayCommand]
        private async Task Create()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description))
            {
                await Shell.Current.DisplayAlert("Error", "Fill all fields", "OK");
                return;
            }

            var dto = new CreateCommunityDto
            {
                Name = Name,
                Description = Description
            };

            var response = await _communityApiService.CreateAsync(dto);

            if (response)
            {
                await Shell.Current.DisplayAlert("Success", "Your community is created", "OK");
                await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Something went wrong try again", "OK");
            }
        }
    }
}
