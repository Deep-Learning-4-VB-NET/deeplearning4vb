Imports MapDBStatsStorage = org.deeplearning4j.ui.model.storage.mapdb.MapDBStatsStorage

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

Namespace org.deeplearning4j.ui.model.storage


	Public Class FileStatsStorage
		Inherits MapDBStatsStorage

		Private ReadOnly file As File

		Public Sub New(ByVal f As File)
			MyBase.New(f)
			Me.file = f
		End Sub

		Public Overrides Function ToString() As String
			Return "FileStatsStorage(" & file.getPath() & ")"
		End Function
	End Class

End Namespace