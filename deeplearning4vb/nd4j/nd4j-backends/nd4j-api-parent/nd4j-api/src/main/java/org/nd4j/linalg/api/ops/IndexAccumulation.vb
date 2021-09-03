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

Namespace org.nd4j.linalg.api.ops

	Public Interface IndexAccumulation
		Inherits Op

		Function validateDataTypes() As Boolean

		''' <summary>
		''' This method returns TRUE if we're going to keep axis, FALSE otherwise
		''' 
		''' @return
		''' </summary>
		ReadOnly Property KeepDims As Boolean

		''' <summary>
		''' This method returns dimensions for this op
		''' @return
		''' </summary>
		Function dimensions() As INDArray

		ReadOnly Property FinalResult As Number
	End Interface

End Namespace