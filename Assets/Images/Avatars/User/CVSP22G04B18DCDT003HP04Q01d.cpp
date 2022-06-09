#include <opencv2/imgcodecs.hpp>
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/objdetect.hpp>
#include <iostream>
using namespace cv;
using namespace std;
int main(void) {
	Mat img,hsv,mask1,mask2,mask, target;
	img =imread("cow.png");
	cvtColor(img, hsv, COLOR_BGR2HSV);
	inRange(hsv, Scalar(36, 0, 0), Scalar(70, 255, 255), mask1);
	inRange(hsv, Scalar(15, 0, 0), Scalar(36, 255, 255), mask2);
	bitwise_or(mask1, mask2, mask);
	bitwise_and(img, img, target, mask = mask);
	imshow("hello", target);
	waitKey(0);
}