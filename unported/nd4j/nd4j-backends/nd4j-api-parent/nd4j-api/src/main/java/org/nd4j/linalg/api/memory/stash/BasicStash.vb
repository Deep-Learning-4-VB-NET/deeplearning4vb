Imports System.Collections.Concurrent
Imports System.Collections.Generic
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

Namespace org.nd4j.linalg.api.memory.stash


	Public MustInherit Class BasicStash(Of T As Object)
		Implements Stash(Of T)

		Protected Friend stash As IDictionary(Of T, INDArray) = New ConcurrentDictionary(Of T, INDArray)()

		Protected Friend Sub New()

		End Sub

		Public Overridable Function checkIfExists(ByVal key As T) As Boolean Implements Stash(Of T).checkIfExists
	'        
	'            Just checkin'
	'         
			Return False
		End Function

		Public Overridable Sub put(ByVal key As T, ByVal [object] As INDArray) Implements Stash(Of T).put
	'        
	'            Basically we want to get DataBuffer here, and store it here together with shape
	'            Special case here is GPU: we want to synchronize HOST memory, and store only HOST memory.
	'         
		End Sub

		Public Overridable Function get(ByVal key As T) As INDArray Implements Stash(Of T).get
	'        
	'            We want to restore INDArray here, In case of GPU backend - we want to ensure data is replicated to device.
	'         
			Return Nothing
		End Function

		Public Overridable Sub purge() Implements Stash(Of T).purge
	'        
	'            We want to purge all stored stuff here.
	'         
		End Sub
	End Class

End Namespace