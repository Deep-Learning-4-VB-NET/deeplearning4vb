Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Reflections = org.reflections.Reflections
Imports MethodAnnotationsScanner = org.reflections.scanners.MethodAnnotationsScanner
Imports ClasspathHelper = org.reflections.util.ClasspathHelper
Imports ConfigurationBuilder = org.reflections.util.ConfigurationBuilder
Imports Test = org.junit.jupiter.api.Test

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
Namespace org.nd4j.common.tests


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class AbstractAssertTestsClass extends BaseND4JTest
	Public MustInherit Class AbstractAssertTestsClass
		Inherits BaseND4JTest

		Protected Friend MustOverride ReadOnly Property Exclusions As ISet(Of [Class])

		Protected Friend MustOverride ReadOnly Property PackageName As String

		Protected Friend MustOverride ReadOnly Property BaseClass As Type

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 240000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void checkTestClasses()
		Public Overridable Sub checkTestClasses()
			Dim reflections As New Reflections((New ConfigurationBuilder()).setUrls(ClasspathHelper.forPackage(PackageName)).setScanners(New MethodAnnotationsScanner()))
			Dim methods As ISet(Of System.Reflection.MethodInfo) = reflections.getMethodsAnnotatedWith(GetType(Test))
			Dim s As ISet(Of Type) = New HashSet(Of Type)()
			For Each m As System.Reflection.MethodInfo In methods
				s.Add(m.getDeclaringClass())
			Next m

			Dim l As IList(Of Type) = New List(Of Type)(s)
			l.Sort(New ComparatorAnonymousInnerClass(Me))

			Dim count As Integer = 0
			For Each c As Type In l
				If Not BaseClass.IsAssignableFrom(c) AndAlso Not getExclusions().Contains(c) Then
					log.error("Test {} does not extend {} (directly or indirectly). All tests must extend this class for proper memory tracking and timeouts", c, BaseClass)
					count += 1
				End If
			Next c
			'assertEquals("Number of tests not extending BaseND4JTest", 0, count);
		End Sub

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Type)

			Private ReadOnly outerInstance As AbstractAssertTestsClass

			Public Sub New(ByVal outerInstance As AbstractAssertTestsClass)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal aClass As Type, ByVal t1 As Type) As Integer Implements IComparer(Of [Class]).Compare
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Return String.CompareOrdinal(aClass.FullName, t1.FullName)
			End Function
		End Class
	End Class

End Namespace