import React, { useState } from 'react';
import RatingService from '../Services/RatingService';
import '../Style/UserRatingComponent.css';

const UserRatingComponent = () => {
  const [rating, setRating] = useState(0);
  const [submitted, setSubmitted] = useState(false);

  const handleRatingChange = (event) => {
    setRating(event.target.value);
  };

  const handleSubmit = async () => {
    try {
      await RatingService.submitRating(rating);
      setSubmitted(true);
    } catch (error) {
      console.error('Error submitting rating:', error);
    }
  };

  return (
    <div className="user-rating-component">
      <h1>Thank you for riding with us!</h1>
      {submitted ? (
        <h2>Your rating has been submitted.</h2>
      ) : (
        <>
          <h2>Please rate your ride experience:</h2>
          <div className="rating-input">
            <input
              type="number"
              min="1"
              max="5"
              value={rating}
              onChange={handleRatingChange}
            />
          </div>
          <button onClick={handleSubmit}>Submit Rating</button>
        </>
      )}
    </div>
  );
};

export default UserRatingComponent;
