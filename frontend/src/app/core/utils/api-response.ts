type ApiListResponse<T> = {
  items?: T[];
  Items?: T[];
  data?: T[];
  Data?: T[];
  value?: T[];
  Value?: T[];
};

export function toApiList<T>(response: T[] | ApiListResponse<T>): T[] {
  if (Array.isArray(response)) {
    return response;
  }

  const candidates = [
    response.items,
    response.Items,
    response.data,
    response.Data,
    response.value,
    response.Value,
  ];
  const items = candidates.find(Array.isArray);
  if (items) {
    return items;
  }

  throw new Error('The API returned an unsupported list response.');
}
