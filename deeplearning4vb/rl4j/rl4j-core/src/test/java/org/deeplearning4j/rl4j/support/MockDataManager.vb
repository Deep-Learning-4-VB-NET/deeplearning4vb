Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.learning
Imports IDataManager = org.deeplearning4j.rl4j.util.IDataManager

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

Namespace org.deeplearning4j.rl4j.support


	Public Class MockDataManager
		Implements IDataManager

		Private ReadOnly isSaveData As Boolean
		Public statEntries As IList(Of StatEntry) = New List(Of StatEntry)()
		Public isSaveDataCallCount As Integer = 0
		Public getVideoDirCallCount As Integer = 0
		Public writeInfoCallCount As Integer = 0
		Public saveCallCount As Integer = 0

		Public Sub New(ByVal isSaveData As Boolean)
			Me.isSaveData = isSaveData
		End Sub

		Public Overridable ReadOnly Property SaveData As Boolean Implements IDataManager.isSaveData
			Get
				isSaveDataCallCount += 1
				Return isSaveData
			End Get
		End Property

		Public Overridable ReadOnly Property VideoDir As String Implements IDataManager.getVideoDir
			Get
				getVideoDirCallCount += 1
				Return Nothing
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void appendStat(StatEntry statEntry) throws java.io.IOException
		Public Overridable Sub appendStat(ByVal statEntry As StatEntry) Implements IDataManager.appendStat
			statEntries.Add(statEntry)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeInfo(org.deeplearning4j.rl4j.learning.ILearning iLearning) throws java.io.IOException
		Public Overridable Sub writeInfo(ByVal iLearning As ILearning) Implements IDataManager.writeInfo
			writeInfoCallCount += 1
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void save(org.deeplearning4j.rl4j.learning.ILearning learning) throws java.io.IOException
		Public Overridable Sub save(ByVal learning As ILearning) Implements IDataManager.save
			saveCallCount += 1
		End Sub
	End Class

End Namespace