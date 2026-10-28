import { DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { ApiService } from './services/api.service';
import { SAMPLE_TYPES, SampledListItem, TEST_CODES } from './models/model';

@Component({
  selector: 'app-root',
  imports: [FormsModule, DatePipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  private api = inject(ApiService);

  sampleTypes = SAMPLE_TYPES;
  testCodes = TEST_CODES;

  samples: SampledListItem[] = [];
  page = 1;
  pageSize = 3;
  totalCount = 0;
  error = '';

  form = this.emptyForm();

  private emptyForm() {
    return {
      documentNumber: '',
      fullName: '',
      sampleType: 'Sangre',
      collectedAt: '',
      testCodes: [] as string[],
    };
  }

  ngOnInit() {
    this.load();
  }

  create() {
    this.api.create(this.form).subscribe({
      next: (value) => {
        this.form = this.emptyForm();
        this.page = 1;
        this.load();
      },
      error(err) {
        alert(err.error.message);
      },
    });
  }

  load() {
    this.api.list(this.page, this.pageSize).subscribe({
      next: (value) => {
        this.samples = value.items;
        this.totalCount = value.totalCount;
      },
      error(err) {
        alert(err.error.message);
      },
    });
  }

  changeStatus(s: SampledListItem, status: string) {
    let rejectionReason: string | null = null;

    if (status == 'Rechazada') {
      rejectionReason = prompt('Motivo del rechazo:');
      if (!rejectionReason?.trim()) return;
    }


    this.api.changeStatus(s.id, {status: status, rejectionReason: rejectionReason}).subscribe({
      next:(value) =>{
        this.load();
      },
      error(err) {
        alert(err.error.message);
      },
    })
  }
}
