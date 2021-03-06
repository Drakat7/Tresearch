import React, { useState } from "react";
import {useNavigate} from "react-router-dom";
import axios, {AxiosResponse, AxiosError} from 'axios';

import "./NodeCreationForm.css";
import Button from "../Button/ButtonComponent";

class NodeCreationForm extends React.Component{
    constructor(props){
        super(props);
        this.state = {
            userhash: props.cRForm.userHash,
            nodeParentID: props.cRForm.nodeID,
            nodeTitle: '',
            summary: ''
        }
        console.log(props.cRForm)
    }


    

    componentDidMount(){
        
    }

    handleInput() {
        return true;
    }

    inputUserHashHandler = (e) => {
        let updatedHash = e.target.value;
        this.setState({ userhash: updatedHash});
    }

    inputParentNodeIDHandler = (e) => {
        let updateParentID = e.target.value;
        this.setState({nodeParentID: updateParentID});
    }
 
    inputTitleHandler = (e) => {
        let updatedTitle = e.target.value;
        this.setState({nodeTitle: updatedTitle});
    }
    
    inputSummaryHandler = (e) => {
        let updatedSummary = e.target.value;
        this.setState({summary: updatedSummary});
    }

    onSubmitHandler = (e) => {
        e.preventDefault();
        
        if(this.handleInput()){
            this.setState({errorMessage: ''})
            axios.post('https://trialbyfiretresearchwebapi.azurewebsites.net/CreateNode/createNode?userhash=' + this.state.userhash + '&parentNodeID=' + 
            this.state.nodeParentID + '&nodeTitle=' + this.state.nodeTitle + '&summary=' + this.state.summary).
            then(response => {
                const responseData = Object.values(response.data);
                window.location.reload();
            })
            .catch(function (error) {
                if (error.response) {
                  // Request made and server responded
                  console.log(error.response.data);
                  console.log(error.response.status);
                  console.log(error.response.headers);
                } else if (error.request) {
                  // The request was made but no response was received
                  console.log(error.request);
                } else {
                  // Something happened in setting up the request that triggered an Error
                  console.log('Error', error.message);}
                })
        }
    }

    render() {
        const renderForm = (
            <div className="form-createNode-container">
                <form className="createNode-form" onSubmit={this.onSubmitHandler}>
                    <div className="input-container">
                        <input type="text" value={this.state.nodeTitle} required placeholder="Node Title" on onChange={this.inputTitleHandler}/>
                    </div>   
                    <div className="input-container">
                        <input type="text" value={this.state.summary} required placeholder="Node Summary" on onChange={this.inputSummaryHandler}/>
                    </div>
                    <div className="create-button-container">
                            <Button type="button" color="green" name="Create"/>
                        </div>
                    {this.state.errorMessage && <div className="error-createNode"> {this.state.errorMessage} </div>}
                </form>
            </div>
        );

        return (
            <div className="form-createNode-wrapper">
                <div className="container-createNode-text">
                    <h1 className="createNode-title">Create Node</h1>
                </div>    
                {renderForm}
            </div>
        );
    }
}

export default(NodeCreationForm);