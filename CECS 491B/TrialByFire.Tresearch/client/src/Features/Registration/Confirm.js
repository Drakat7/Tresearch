import React from "react";
import { useParams,} from "react-router-dom";
import NavigationBar from "../../UI/Navigation/NavigationBar";
import "./Confirm.css";
import axios, {AxiosResponse, AxiosError} from 'axios';



class Confirm extends React.Component {
     render() {
        function GetGuid() {
            const { confirmationGuid } = useParams();
            console.log(confirmationGuid);
            if(confirmationGuid != null){
                axios.post('https://localhost:7010/Registration/confirm?'+confirmationGuid)
                .then(res => {
                    window.location = '/Register/AccountConfirmed';
                    return res;
                })
                .catch( err => {
                    //window.location = '/Register/EULATerms';
                    console.log(err);
                    return err;
                })
            }
            return null;
        }

        const renderConfirm = (
          <div className = "register-confirm-container">
              <p><GetGuid/></p>
          </div>
        )
      return (
          <div className = "register-confirm-wrapper">
            {renderConfirm}
          </div>
      );
  }
}

export default Confirm;