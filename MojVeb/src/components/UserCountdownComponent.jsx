import React, { useEffect, useState } from 'react';
import CountdownService from '../Services/CountdownService';
import RatingService from '../Services/RatingService';
import '../Style/UserCountdownComponent.css';

const UserCountdownComponent = () => {
  const [countdown, setCountdown] = useState({ waitDuration: 0, rideDuration: 0, isAccepted: '' });
  const [rideEnded, setRideEnded] = useState(false);
  const [rating, setRating] = useState(0);
  const [driverEmail, setDriverEmail] = useState('');

  useEffect(() => {
    let isMounted = true;

    const fetchCountdown = async () => {
      try {
        const data = await CountdownService.getCountdown();
        console.log('Fetched countdown:', data); // Debug: Log the fetched data
        if (isMounted) {
          setCountdown(data);
          if (data.rideDuration <= 5) {
            setRideEnded(true);
            setDriverEmail(data.isAccepted); // Set the driverEmail from fetched data
          }
        }
      } catch (error) {
        console.error('Error fetching countdown:', error);
      }
    };

    fetchCountdown(); // Initial fetch

    const interval = setInterval(fetchCountdown, 1000);

    return () => {
      isMounted = false;
      clearInterval(interval);
    };
  }, []);

  const handleRatingChange = (event) => {
    setRating(event.target.value);
  };

  const handleSubmitRating = async () => {
    try {
      await RatingService.submitRating({ rating, isAccepted: driverEmail });
      alert('Rating submitted successfully!');
    } catch (error) {
      console.error('Error submitting rating:', error);
      alert('Failed to submit rating.');
    }
  };

  if (rideEnded) {
    return (
      <div className="user-countdown-component">
        <h1>Thank you for riding with us!</h1>
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
        <button onClick={handleSubmitRating}>Submit Rating</button>
      </div>
    );
  }

  return (
    <div className="user-countdown-component">
      <h1>Countdown Timer</h1>
      <div className="countdown-timers">
        <div>
          <h2>Wait Duration: {countdown.waitDuration}</h2>
        </div>
        <div>
          <h2>Ride Duration: {countdown.rideDuration}</h2>
        </div>
      </div>
    </div>
  );
};

export default UserCountdownComponent;
