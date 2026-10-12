Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater

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

Namespace org.nd4j.linalg.learning


	Public Interface GradientUpdater(Of T As org.nd4j.linalg.learning.config.IUpdater)

		ReadOnly Property Config As T

		Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean)

		ReadOnly Property State As IDictionary(Of String, INDArray)

		''' <summary>
		''' For the internal updater state (if any): set this to use the provided array.
		''' Used during initialization, and when restoring the updater state (after serialization, for example) </summary>
		'''  <param name="viewArray">    Array (that is a view of a larger array) to use for the state. </param>
		''' <param name="gradientShape"> </param>
		''' <param name="gradientOrder"> </param>
		''' <param name="initialize">   If true: the updater must initialize the view array. If false: no change to view array contents </param>
		Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean)

		''' <summary>
		''' Modify the gradient to be an update. Note that this is be done in-place
		''' </summary>
		''' <param name="gradient">  the gradient to modify </param>
		''' <param name="iteration"> </param>
		''' <returns> the modified gradient </returns>
		Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer)
	End Interface

End Namespace