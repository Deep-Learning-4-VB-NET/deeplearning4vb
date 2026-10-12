Imports System
Imports System.Collections.Concurrent
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Storage = org.nd4j.parameterserver.distributed.logic.Storage

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

Namespace org.nd4j.parameterserver.distributed.logic.storage


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public abstract class BaseStorage implements org.nd4j.parameterserver.distributed.logic.Storage
	<Obsolete>
	Public MustInherit Class BaseStorage
		Implements Storage

		Public MustOverride Function arrayExists(ByVal key As Integer?) As Boolean Implements Storage.arrayExists
		Public MustOverride Sub setArray(ByVal key As Integer?, ByVal array As INDArray) Implements Storage.setArray
		Public MustOverride Function getArray(ByVal key As Integer?) As INDArray Implements Storage.getArray

		Private storage As New ConcurrentDictionary(Of Integer, INDArray)()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray getArray(@NonNull Integer key)
		Public Overridable Function getArray(ByVal key As Integer) As INDArray
			Return storage(key)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setArray(@NonNull Integer key, @NonNull INDArray array)
		Public Overridable Sub setArray(ByVal key As Integer, ByVal array As INDArray)
			storage(key) = array
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public boolean arrayExists(@NonNull Integer key)
		Public Overridable Function arrayExists(ByVal key As Integer) As Boolean
			Return storage.ContainsKey(key)
		End Function

		Public Overridable Sub shutdown() Implements Storage.shutdown
			storage.Clear()
		End Sub
	End Class

End Namespace