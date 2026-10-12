Imports System
Imports System.Collections.Generic
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports NoOp = org.nd4j.linalg.api.ops.NoOp
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports Reflections = org.reflections.Reflections
Imports SubTypesScanner = org.reflections.scanners.SubTypesScanner
Imports ClasspathHelper = org.reflections.util.ClasspathHelper
Imports ConfigurationBuilder = org.reflections.util.ConfigurationBuilder
Imports FilterBuilder = org.reflections.util.FilterBuilder
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.ops


	Public Class OpConstructorTests
		Inherits BaseNd4jTestWithBackends

		'Ignore individual classes
		Protected Friend exclude As ISet(Of Type) = New HashSet(Of Type)(java.util.Arrays.asList(GetType(NoOp)))

		'Ignore whole sets of classes based on regex
		Protected Friend ignoreRegexes() As String = { "org\.nd4j\.linalg\.api\.ops\.impl\.controlflow\..*" }

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("Need to check") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void checkForINDArrayConstructors(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub checkForINDArrayConstructors(ByVal backend As Nd4jBackend)
	'        
	'        Check that all op classes have at least one INDArray or INDArray[] constructor, so they can actually
	'        be used outside of SameDiff
	'         

			Dim f As New Reflections((New ConfigurationBuilder()).filterInputsBy((New FilterBuilder()).include(FilterBuilder.prefix("org.nd4j.*")).exclude("^(?!.*\.class$).*$")).setUrls(ClasspathHelper.forPackage("org.nd4j")).setScanners(New SubTypesScanner()))

			Dim classSet As ISet(Of Type) = f.getSubTypesOf(GetType(DifferentialFunction))

			Dim count As Integer = 0
			Dim classes As IList(Of Type) = New List(Of Type)()
			For Each c As Type In classSet
				If Modifier.isAbstract(c.getModifiers()) OrElse Modifier.isInterface(c.getModifiers()) OrElse c = GetType(SDVariable) OrElse c.IsAssignableFrom(GetType(ILossFunction)) Then
					Continue For
				End If

				If exclude.Contains(c) Then
					Continue For
				End If

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Dim cn As String = c.FullName
				Dim ignored As Boolean = False
				For Each s As String In ignoreRegexes
					If cn.matches(s) Then
						ignored = True
						Exit For
					End If
				Next s
				If ignored Then
					Continue For
				End If

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.lang.reflect.Constructor<?>[] constructors = c.getConstructors();
				Dim constructors() As System.Reflection.ConstructorInfo(Of Object) = c.GetConstructors()
				Dim foundINDArray As Boolean = False
				For i As Integer = 0 To constructors.Length - 1
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.lang.reflect.Constructor<?> co = constructors[i];
					Dim co As System.Reflection.ConstructorInfo(Of Object) = constructors(i)
					Dim str As String = co.toGenericString() 'This is a convenience hack for checking - returns strings like "public org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(org.nd4j.linalg.api.ndarray.INDArray,int...)"
					If str.Contains("INDArray") AndAlso Not str.Contains("SameDiff") Then
						foundINDArray = True
						Exit For
					End If
				Next i

				If Not foundINDArray Then
					classes.Add(c)
				End If
			Next c

			If classes.Count > 0 Then
				classes.Sort(System.Collections.IComparer.comparing(AddressOf Type.getName))
				For Each c As Type In classes
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Console.WriteLine("No INDArray constructor: " & c.FullName)
				Next c
			End If
			assertEquals(0, classes.Count,"Found " & classes.Count & " (non-ignored) op classes with no INDArray/INDArray[] constructors")

		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

	End Class

End Namespace