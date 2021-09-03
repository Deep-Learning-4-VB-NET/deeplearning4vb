﻿Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

'
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

Namespace org.deeplearning4j.zoo.util


	Public Interface Labels

		''' <summary>
		''' Returns the description of the nth class from the classes of a dataset. </summary>
		''' <param name="n"> </param>
		''' <returns> label description </returns>
		Function getLabel(ByVal n As Integer) As String

		''' <summary>
		''' Given predictions from the trained model this method will return a list
		''' of the top n matches and the respective probabilities. </summary>
		''' <param name="predictions"> raw </param>
		''' <returns> decoded predictions </returns>
		Function decodePredictions(ByVal predictions As INDArray, ByVal n As Integer) As IList(Of IList(Of ClassPrediction))
	End Interface

End Namespace