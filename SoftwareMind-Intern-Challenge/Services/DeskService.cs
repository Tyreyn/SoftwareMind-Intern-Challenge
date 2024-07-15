﻿namespace SoftwareMind_Intern_Challenge.Services
{
    using SoftwareMind_Intern_ChallengeBL.Operations;
    using SoftwareMind_Intern_ChallengeDTO.DataObjects;

    /// <summary>
    /// Desk service.
    /// </summary>
    /// <param name="deskOperations">
    /// Available operations for desk model.
    /// </param>
    /// <param name="locationOperations">
    /// Available operations for location model.
    /// </param>
    public class DeskService(DeskOperations deskOperations, LocationOperations locationOperations)
    {
        private readonly DeskOperations deskOperations = deskOperations;

        private readonly LocationOperations locationOperations = locationOperations;

        /// <summary>
        /// Add new desk to location.
        /// </summary>
        /// <param name="locationId">
        /// Location ID where to add desk.
        /// </param>
        /// <returns>
        /// True, if added correctly.
        /// </returns>
        public bool AddDesk(int locationId)
        {
            Desk newDesk = new Desk();
            newDesk.LocationId = locationId;
            newDesk.IsAvailable = locationId == 1 ? false : true;
            this.deskOperations.AddDesk(newDesk);
            return true;
        }

        /// <summary>
        /// Get all desks.
        /// </summary>
        /// <returns>
        /// List of desks.
        /// </returns>
        public IList<Desk>? GetDesks()
        {
            return this.deskOperations.GetAllDesks();
        }

        /// <summary>
        /// Change desk location.
        /// </summary>
        /// <param name="newLocationId">
        /// New location for desk.
        /// </param>
        /// <param name="deskId">
        /// Desk ID that location will be changed.
        /// </param>
        /// <returns>
        /// T1 - true, if desk location changed correctly, otherwise false.
        /// T2 - message.
        /// </returns>
        public (bool, string) ChangeDeskLocation(int newLocationId, int deskId)
        {
            Desk? deskToChange = this.deskOperations.GetDeskById(deskId);
            if (deskToChange == null)
            {
                return (false, "There is no desk with this ID");
            }

            int oldLocationId = deskToChange.LocationId;
            deskToChange.LocationId = newLocationId;
            deskToChange.Location = this.locationOperations.GetLocationById(newLocationId);
            deskToChange.IsAvailable = true;

            bool result = this.deskOperations.UpdateDesk(deskToChange);
            if (!result)
            {
                return (false, "Something went wrong with updating location");
            }

            return (true, $"Location of desk {deskId} changed from {oldLocationId} to {newLocationId}");
        }

        /// <summary>
        /// Change desk availability.
        /// </summary>
        /// <param name="deskId">
        /// ID of desk to be changed.
        /// </param>
        /// <returns>
        /// True, if desk availability changed correctly.
        /// </returns>
        public bool ChangeDeskAvailable(int deskId)
        {
            Desk? deskToBeChanged = this.deskOperations.GetDeskById(deskId);

            if (deskToBeChanged == null)
            {
                return false;
            }

            deskToBeChanged.IsAvailable = !deskToBeChanged.IsAvailable;

            this.deskOperations.UpdateDesk(deskToBeChanged);
            return true;
        }

        /// <summary>
        /// Delete desk.
        /// </summary>
        /// <param name="deskId">
        /// ID of desk to be deleted.
        /// </param>
        /// <returns>
        /// T1 - true, if deleted correctly, otherwise false.
        /// T2 - message.
        /// </returns>
        public (bool, string) DeleteDesk(int deskId)
        {
            Desk? deskToBeDeleted = this.deskOperations.GetDeskById(deskId);

            if (deskToBeDeleted == null)
            {
                return (false, "There is no desks with this ID");
            }
            else if (deskToBeDeleted.Reservations != null
                && deskToBeDeleted.Reservations.Count() > 0)
            {
                return (false, "There is still reservations for this desk");
            }

            this.deskOperations.DeleteDesk(deskToBeDeleted);
            return (true, $"Desk {deskId} deleted successfully");
        }
    }
}
