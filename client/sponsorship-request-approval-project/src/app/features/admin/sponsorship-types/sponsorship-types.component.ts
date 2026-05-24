import { Component, OnInit } from '@angular/core';
import { SponsorshipType } from '../../../core/models/sponsorship-type.models';
import { SponsorshipTypeService } from '../../../core/services/sponsorship-type.service';

@Component({
  selector: 'app-sponsorship-types',
  standalone: true,
  imports: [],
  templateUrl: './sponsorship-types.component.html'
})
export class SponsorshipTypesComponent implements OnInit {
  items: SponsorshipType[] = [];
  constructor(private readonly service: SponsorshipTypeService) {}
  ngOnInit(): void { this.load(); }
  load(): void { this.service.getAll().subscribe(items => this.items = items); }
}
