import {EventEmitter, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ServerStatusService {

  private pollingInterval = 2000;

  public serverAvailable = false;
  public serverAvailabilityChanged = new EventEmitter<boolean>();

  constructor(private http: HttpClient) {

    setInterval(
      () => {
        this.http.get(`${environment.baseHost}/ping`).subscribe(
          res => {
            if (!this.serverAvailable)
            { // If wasn't available, update to true
              this.serverAvailable = true;
              this.serverAvailabilityChanged.emit(this.serverAvailable);
            }
          },
          err => {
            if (this.serverAvailable)
            { // If was available, update to false
              this.serverAvailable = false;
              this.serverAvailabilityChanged.emit(this.serverAvailable);
            }
          }
        )
      },
      this.pollingInterval
    );

  }

}
