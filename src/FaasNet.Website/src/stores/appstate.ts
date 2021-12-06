import { createSelector } from '@ngrx/store';
import * as fromFunctions from './functions/reducers';

export interface AppState {
  functions: fromFunctions.SearchFunctionsState;
  function: fromFunctions.FunctionState;
}

export const selectFunctions = (state: AppState) => state.functions;
export const selectFunction = (state: AppState) => state.function;

export const selectFunctionsResult = createSelector(
  selectFunctions,
  (state: fromFunctions.SearchFunctionsState) => {
    if (!state || state.Functions === null) {
      return null;
    }

    return state.Functions;
  }
);

export const selectFunctionConfigurationResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.Configuration === null) {
      return null;
    }

    return state.Configuration;
  }
);

export const selectFunctionResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.Function === null) {
      return null;
    }

    return state.Function;
  }
);

export const selectFunctionThreadsResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.Threads === null) {
      return null;
    }

    return state.Threads;
  }
);

export const selectFunctionVirtualMemoryBytesResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.VirtualMemoryBytes === null) {
      return null;
    }

    return state.VirtualMemoryBytes;
  }
);

export const selectCpuUsageResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.CpuUsage === null) {
      return null;
    }

    return state.CpuUsage;
  }
);

export const selectRequestDurationResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.RequestDuration === null) {
      return null;
    }

    return state.RequestDuration;
  }
);

export const selectDetailsResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.Details === null) {
      return null;
    }

    return state.Details;
  }
);

export const selectTotalRequests = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.TotalRequests === null) {
      return null;
    }

    return state.TotalRequests;
  }
);

export const appReducer = {
  functions: fromFunctions.getSearchFunctionsReducer,
  function: fromFunctions.getFunctionReducer
};
