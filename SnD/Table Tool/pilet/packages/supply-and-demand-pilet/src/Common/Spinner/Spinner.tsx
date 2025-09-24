import { Component } from 'react';
import { LoaderOverlay, LoaderSpinner } from './Spinner.style';

export default class Spinner extends Component {

    render() {
        return (
          <LoaderOverlay>
            <LoaderSpinner />
          </LoaderOverlay>
        )
    };
}
