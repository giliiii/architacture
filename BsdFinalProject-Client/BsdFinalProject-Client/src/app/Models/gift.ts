
export class GiftModel {
  id!: Number;
  name!: string;
  description?: string;
  cost!: Number;
  picture?: string;
  categoryId!: Number;
  donorId!: Number;
  winnerName?: string;
}