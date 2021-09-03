﻿Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports AbstractAssertTestsClass = org.nd4j.common.tests.AbstractAssertTestsClass

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
Namespace org.deeplearning4j

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class AssertTestsExtendBaseClass extends org.nd4j.common.tests.AbstractAssertTestsClass
	Public Class AssertTestsExtendBaseClass
		Inherits AbstractAssertTestsClass

		Protected Friend Overrides ReadOnly Property Exclusions As ISet(Of [Class])
			Get
				Dim exclusions As ISet(Of Type) = New HashSet(Of Type)()
				Return exclusions
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property PackageName As String
			Get
				Return "org.deeplearning4j"
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property BaseClass As Type
			Get
				Return GetType(BaseDL4JTest)
			End Get
		End Property
	End Class


End Namespace