﻿'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.autodiff.loss

	Public Enum LossReduce
		''' <summary>
		''' No reduction. In most cases, output is the same shape as the predictions/labels.<br>
		''' Weights (if any) are applied<br>
		''' Example Input: 2d input array with mean squared error loss.<br>
		''' Example Output: squaredDifference(predictions,labels), with same shape as input/labels<br>
		''' </summary>
		NONE

		''' <summary>
		''' Weigted sum across all loss values, returning a scalar.<br>
		''' </summary>
		SUM

		''' <summary>
		''' Weighted mean: sum(weights * perOutputLoss) / sum(weights) - gives a single scalar output<br>
		''' Example: 2d input, mean squared error<br>
		''' Output: squared_error_per_ex = weights * squaredDifference(predictions,labels)<br>
		'''         output = sum(squared_error_per_ex) / sum(weights)<br>
		''' <br>
		''' NOTE: if weights array is not provided, then weights default to 1.0 for all entries - and hence
		''' MEAN_BY_WEIGHT is equivalent to MEAN_BY_NONZERO_WEIGHT_COUNT
		''' </summary>
		MEAN_BY_WEIGHT

		''' <summary>
		''' Weighted mean: sum(weights * perOutputLoss) / count(weights != 0)<br>
		''' Example: 2d input, mean squared error loss.<br>
		''' Output: squared_error_per_ex = weights * squaredDifference(predictions,labels)<br>
		'''         output = sum(squared_error_per_ex) /  count(weights != 0)<br>
		''' 
		''' NOTE: if weights array is not provided, then weights default to scalar 1.0 and hence MEAN_BY_NONZERO_WEIGHT_COUNT
		''' is equivalent to MEAN_BY_WEIGHT
		''' </summary>
		MEAN_BY_NONZERO_WEIGHT_COUNT
	End Enum

End Namespace